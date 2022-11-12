using pwnwrk.infra.Exceptions;
using pwnwrk.infra.Queues;
using pwnwrk.infra.Repositories;
using pwnwrk.infra.Extensions;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Persistence.Extensions;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Notifications;
using pwnwrk.domain.Notifications.Entities;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Tasks.Entities;
using pwnwrk.domain.Assets.BaseClasses;
using Microsoft.EntityFrameworkCore;

namespace pwnwrk.infra.Utilities
{
    public sealed class AssetProcessor
    {
        private readonly AssetRepository _assetRepo = new();
        private readonly NotificationSender _notificationSender = new();
        private static readonly IJobQueueService _jobQueueService = JobQueueFactory.Create();
        private readonly PwnctlDbContext _context = new();
        private readonly List<TaskDefinition> _taskDefinitions;
        private readonly List<NotificationRule> _notificationRules;
        private readonly List<Program> _programs;

        public AssetProcessor()
        {
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

            var program = asset.GetOwningProgram(_programs);

            await _assetRepo.SaveAsync(asset);
            
            if (!asset.InScope)
            {
                return;
            }

            foreach (var rule in _notificationRules.Where(rule => rule.Check(asset)))
            {
                _notificationSender.Send(asset, rule);
            }

            var allowedTaskDefinitions = program.GetAllowedTasks(_taskDefinitions, asset.GetType());

            foreach(var definition in allowedTaskDefinitions.Where(def => def.Matches(asset)))
            {
                // only queue tasks once per definition/asset pair
                var task = _context.FindAssetTaskRecord(asset, definition);
                if (task != null)
                    continue;

                task = new TaskRecord(definition, asset);

                await _jobQueueService.EnqueueAsync(task);
            }

            await _assetRepo.SaveAsync(asset);
        }
    }
}
