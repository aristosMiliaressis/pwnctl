using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.kernel.Extensions;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Common.Interfaces;

namespace pwnctl.app.Assets
{
    public sealed class AssetProcessor
    {
        private readonly AssetRepository _assetRepository;
        private readonly List<TaskDefinition> _taskDefinitions;
        private readonly List<NotificationRule> _notificationRules;
        private readonly List<Program> _programs;

        public AssetProcessor(AssetRepository assetRepository, 
            List<TaskDefinition> definitions, List<NotificationRule> rules, List<Program> programs)
        {
            _assetRepository = assetRepository;
            _taskDefinitions = definitions;
            _notificationRules = rules;
            _programs = programs;
        }

        public async Task ProcessAsync(string assetText)
        {
            Dictionary<string, object> tags = null;
            if (assetText.StartsWith("{"))
            {
                var dto = Serializer.Instance.Deserialize<AssetDTO>(assetText);
                assetText = dto.Asset;
                tags = dto.Tags;
            }

            Asset asset = AssetParser.Parse(assetText);

            await ProcessAssetAsync(asset, tags);
        }

        private async Task ProcessAssetAsync(Asset asset, Dictionary<string, object> tags)
        {
            // recursivly process all parsed assets
            // starting from the botton of the ref tree.
            await asset.GetType()
                    .GetProperties()
                    .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)))
                    .Select(rf => (Asset)rf.GetValue(asset))
                    .Where(a => a != null)
                    .ForEachAsync(async refAsset =>
                    {
                        await ProcessAssetAsync(refAsset, null);
                    });

            var record = await _assetRepository.FindRecordAsync(asset); 
            if (record == null)
            {
                record = new AssetRecord(asset);
            }

            record = await _assetRepository.MergeCurrentRecordWithDBRecord(record, asset);
            var owningProgram = _programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(asset)));

            record.SetOwningProgram(owningProgram);
            record.UpdateTags(tags);

            if (record.InScope)
            {
                ProcessInScopeAsset(record);
            }

            await _assetRepository.SaveAsync(record);
        }

        private void ProcessInScopeAsset(AssetRecord record)
        {
            foreach (var rule in _notificationRules.Where(rule => rule.Check(record)))
            {
                NotificationSender.Instance.Send(record.Asset, rule);
            }

            var matchingTasks = record.OwningProgram.Policy
                                    .GetAllowedTasks(_taskDefinitions)
                                    .Where(def => def.Matches(record));

            foreach (var definition in matchingTasks)
            {
                // only queue tasks once per definition/asset pair
                var task = _assetRepository.FindTaskEntry(record.Asset, definition);
                if (task != null)
                    continue;

                record.Tasks.Add(new TaskEntry(definition, record));
            }
        }
    }
}
