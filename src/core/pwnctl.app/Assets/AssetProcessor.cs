using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.kernel.Extensions;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Assets.DTO;

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

        public async Task<bool> TryProcessAsync(string assetText)
        {
            try
            {
                await ProcessAsync(assetText);
                return true;
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
                return false;
            }
        }

        public async Task ProcessAsync(string assetText)
        {
            AssetDTO dto = TagParser.Parse(assetText);

            Asset asset = AssetParser.Parse(dto.Asset);

            // this is done twice to ...
            await ProcessAssetAsync(asset, dto.Tags, dto.FoundBy); // TODO: do this in a loop
            await ProcessAssetAsync(asset, dto.Tags, dto.FoundBy);
        }

        private async Task ProcessAssetAsync(Asset asset, Dictionary<string, object> tags, string foundBy, List<string> refChain = null)
        {
            refChain = refChain == null
                    ? new List<string>()
                    : new List<string>(refChain);

            // if type exists in chain return to prevent infinit loop
            if (refChain.Contains(asset.Id))
                return;
            refChain.Add(asset.Id);

            // recursivly process all parsed assets
            // starting from the botton of the ref tree.
            await GetReferencedAssets(asset)
                    .ForEachAsync(async refAsset =>
                    {
                        await ProcessAssetAsync(refAsset, null, foundBy, refChain);
                    });

            var record = await _assetRepository.FindRecordAsync(asset); 
            if (record == null)
            {
                record = new AssetRecord(asset, foundBy);
            }

            record = await _assetRepository.MergeCurrentRecordWithDBRecord(record, asset);

            record.UpdateTags(tags);

            var owningProgram = _programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(record.Asset)));
            record.SetOwningProgram(owningProgram);
            
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
                PwnInfraContext.NotificationSender.Send(record.Asset, rule);
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
    
        private List<Asset> GetReferencedAssets(Asset asset)
        {
            var assetProperties = asset.GetType().GetProperties();
            List<Asset> assets = assetProperties
                   .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)))
                   .Select(rf => (Asset)rf.GetValue(asset))
                   .Where(a => a != null)
                   .ToList();

            assets.AddRange(assetProperties
                   .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.GetGenericArguments()[0].IsAssignableTo(typeof(Asset))
                            && p.GetValue(asset) != null)
                   .SelectMany(rf => (IEnumerable<Asset>)rf.GetValue(asset)));

            return assets;
        }
    }
}
