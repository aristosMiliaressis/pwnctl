using pwnwrk.infra.Repositories;
using pwnwrk.infra.Extensions;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Persistence.Extensions;
using pwnwrk.infra.Logging;
using pwnwrk.domain.Notifications.Entities;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Tasks.Entities;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.Exceptions;
using pwnwrk.domain.Assets.Interfaces;
using pwnwrk.domain.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using pwnwrk.domain.Notifications.Interfaces;

namespace pwnwrk.infra.Utilities
{
    public sealed class AssetProcessor
    {
        private readonly AssetRepository _assetRepo = new AssetDbRepository();
        private readonly JobQueueService _jobQueueService;
        private readonly PwnctlDbContext _context = new();
        private readonly List<TaskDefinition> _taskDefinitions;
        private readonly List<NotificationRule> _notificationRules;
        private readonly List<Program> _programs;

        public AssetProcessor(JobQueueService jobQueueService)
        {
            _jobQueueService = jobQueueService;

            _programs = _context.ListPrograms();

            _taskDefinitions = _context.TaskDefinitions.ToList();

            _notificationRules = _context.NotificationRules.AsNoTracking().ToList();
        }


        public async Task<bool> TryProccessAsync(string assetText)
        {
            try
            {
                await ProcessAsync(assetText);
                return true;
            }
            catch (Exception ex)
            {
                PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                return false;
            }
        }

        public async Task ProcessAsync(string assetText)
        {
            bool parsed = AssetParser.TryParse(assetText, out Type[] assetTypes, out Asset[] assets);
            if (!parsed)
            {
                throw new UnparsableAssetException(assetText);
            }

            foreach (var asset in assets)
            {
                await HandleAssetAsync((Asset)asset);
            }
        }

        private async Task HandleAssetAsync(Asset asset)
        {
            // recursivly process all parsed assets
            // starting from the botton of the ref tree.
            await asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)))
                .Select(rf => (Asset) rf.GetValue(asset))
                .Where(a => a != null)
                .ToList()
                .ForEachAsync(async refAsset =>
                {
                    await HandleAssetAsync(refAsset);
                });

            asset = await _assetRepo.LoadRelatedAssets(asset);

            var program = asset.GetOwningProgram(_programs);

            await _assetRepo.SaveAsync(asset);
            
            if (!asset.InScope)
            {
                return;
            }

            foreach (var rule in _notificationRules.Where(rule => rule.Check(asset)))
            {
                NotificationSender.Instance.Send(asset, rule);
            }

            var matchingTasks = program.Policy
                                    .GetAllowedTasks(_taskDefinitions)
                                    .Where(def => def.Matches(asset));

            foreach(var definition in matchingTasks)
            {
                // only queue tasks once per definition/asset pair
                var task = _context.FindAssetTaskRecord(asset, definition);
                if (task != null)
                    continue;

                task = new TaskRecord(definition, asset);
                
                _context.Entry(task).State = EntityState.Added;
                await _context.SaveChangesAsync();

                await _jobQueueService.EnqueueAsync(task);
            }

            await _context.SaveChangesAsync();
        }
    }
}
