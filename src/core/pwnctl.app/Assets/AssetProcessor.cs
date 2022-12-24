using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.kernel.Extensions;
using pwnctl.app.Assets.Aggregates;

namespace pwnctl.app.Assets
{
    public sealed class AssetProcessor
    {
        private readonly AssetRepository _assetRepository;
        private readonly TaskQueueService _taskQueueService;
        private readonly List<TaskDefinition> _taskDefinitions;
        private readonly List<NotificationRule> _notificationRules;

        public AssetProcessor(TaskQueueService TaskQueueService, AssetRepository assetRepository, 
            List<TaskDefinition> definitions, List<NotificationRule> rules)
        {
            _taskQueueService = TaskQueueService;
            _assetRepository = assetRepository;
            _taskDefinitions = definitions;
            _notificationRules = rules;
        }

        public async Task ProcessAsync(string assetText)
        {
            Asset[] assets = AssetParser.Parse(assetText, out Type[] assetTypes);

            foreach (var asset in assets)
            {
                await ProcessAssetAsync(asset);
            }
        }

        private async Task ProcessAssetAsync(Asset asset)
        {
            // recursivly process all parsed assets
            // starting from the botton of the ref tree.
            await asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)))
                .Select(rf => (Asset) rf.GetValue(asset))
                .Where(a => a != null)
                .ForEachAsync(async refAsset =>
                {
                    await ProcessAssetAsync(refAsset);
                });

            var record = await _assetRepository.GetAssetRecord(asset);
            await _assetRepository.SaveAsync(record);

            if (record.Asset.InScope)
            {
                await ProcessInScopeAssetAsync(record);           
            }

            await _assetRepository.SaveAsync(record);
        }

        private async Task ProcessInScopeAssetAsync(AssetRecord record)
        {
            foreach (var rule in _notificationRules.Where(rule => rule.Check(record.Asset)))
            {
                NotificationSender.Instance.Send(record.Asset, rule);
            }

            var matchingTasks = record.OwningProgram.Policy
                                    .GetAllowedTasks(_taskDefinitions)
                                    .Where(def => def.Matches(record.Asset));

            foreach (var definition in matchingTasks)
            {
                // only queue tasks once per definition/asset pair
                var task = _assetRepository.FindTaskRecord(record.Asset, definition);
                if (task != null)
                    continue;

                task = new TaskRecord(definition, record);

                await _assetRepository.SaveAsync(record);

                bool queued = await _taskQueueService.EnqueueAsync(task.ToDTO());
                if (queued)
                    task.Queued();
            }
        }
    }
}
