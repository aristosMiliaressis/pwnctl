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
            _taskDefinitions = _context.TaskDefinitions
                                            .ToList();

            _programs = _context.ListPrograms();

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

            await _assetRepo.AddOrUpdateAsync(asset);

            // load asset references to be used in scope checking process
            asset = await _assetRepo.GetAssetWithReferencesAsync(asset);

            var program = asset.GetOwningProgram(_programs);
            if (program == null)
            {
                return;
            }

            var notifications = asset.GetMatchingNotificationRules(_notificationRules);
            foreach (var notification in notifications)
            {
                _notificationSender.Send(asset, notification);
            }

            var allowedTaskDefinitions = program.AllowedTasks(_taskDefinitions, asset.GetType());

            var matchingTasks = asset.GetMatchingTaskDefinitions(allowedTaskDefinitions);
            foreach(var definition in matchingTasks)
            {
                // only queue tasks once per Taskdefinition/Asset pair
                var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset, definition);
                var task = (TaskRecord)_context.FirstFromLambda(lambda);
                if (task != null)
                    continue;

                task = new TaskRecord(definition, asset);
                _context.TaskRecords.Add(task);

                await _context.SaveChangesAsync();
                
                await _jobQueueService.EnqueueAsync(task);
            }
            
            await _assetRepo.UpdateAsync(asset);
        }
    }
}
