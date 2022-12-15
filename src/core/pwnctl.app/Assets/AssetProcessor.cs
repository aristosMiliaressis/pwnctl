using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.kernel.Extensions;

namespace pwnctl.app.Assets
{
    public sealed class AssetProcessor
    {
        private readonly AssetRepository _assetRepository;
        private readonly TaskQueueService _TaskQueueService;
        private readonly List<TaskDefinition> _taskDefinitions;
        private readonly List<NotificationRule> _notificationRules;
        private readonly List<Program> _programs;

        public AssetProcessor(TaskQueueService TaskQueueService, AssetRepository assetRepository, 
            List<TaskDefinition> definitions, List<NotificationRule> rules, List<Program> programs)
        {
            _TaskQueueService = TaskQueueService;
            _assetRepository = assetRepository;
            _programs = programs;
            _taskDefinitions = definitions;
            _notificationRules = rules;
        }

        public async Task ProcessAsync(string assetText)
        {
            Asset[] assets = AssetParser.Parse(assetText, out Type[] assetTypes);

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

            var assetRecord = await _assetRepository.LoadRelatedAssets(asset);

            var program = _programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(assetRecord.Asset)));

            assetRecord.Asset.InScope = program != null;

            await _assetRepository.SaveAsync(assetRecord);
            
            if (!assetRecord.Asset.InScope)
            {
                return;
            }

            foreach (var rule in _notificationRules.Where(rule => rule.Check(assetRecord.Asset)))
            {
                NotificationSender.Instance.Send(assetRecord.Asset, rule);
            }

            var matchingTasks = program.Policy
                                    .GetAllowedTasks(_taskDefinitions)
                                    .Where(def => def.Matches(assetRecord.Asset));

            foreach(var definition in matchingTasks)
            {
                // only queue tasks once per definition/asset pair
                var task = _assetRepository.FindTaskRecord(assetRecord.Asset, definition);
                if (task != null)
                    continue;

                task = new TaskRecord(definition, assetRecord);

                await _assetRepository.SaveAsync(assetRecord);

                await _TaskQueueService.EnqueueAsync(task);
            }

            await _assetRepository.SaveAsync(assetRecord);
        }
    }
}
