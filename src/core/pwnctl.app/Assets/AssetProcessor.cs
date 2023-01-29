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

        public AssetProcessor(AssetRepository assetRepository, List<Program> programs,
                            List<TaskDefinition> definitions, List<NotificationRule> rules)
        {
            _assetRepository = assetRepository;
            _taskDefinitions = definitions;
            _notificationRules = rules;
            _programs = programs;
        }

        public async Task<bool> TryProcessAsync(string assetText, TaskDefinition definition = null)
        {
            try
            {
                await ProcessAsync(assetText, definition);
                return true;
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
                return false;
            }
        }

        public async Task ProcessAsync(string assetText, TaskDefinition definition = null)
        {
            AssetDTO dto = TagParser.Parse(assetText);

            Asset asset = AssetParser.Parse(dto.Asset);

            string foundBy = definition?.ShortName ?? "N/A";

            // The desired traversal of the following reference sub-tree is (B-C-B-A)
            // the current solution results in the traversal (B-C-A-B-C-A) which is suboptimal
            //    A
            //   / \
            //  B   C
            // TODO: optimize & decuple the reference graph traversal, from the asset processing
            await ProcessAssetAsync(asset, dto.Tags, foundBy);
            await ProcessAssetAsync(asset, dto.Tags, foundBy);
        }

        private async Task ProcessAssetAsync(Asset asset, Dictionary<string, object> tags, string foundBy, List<string> refChain = null)
        {
            refChain = refChain == null
                    ? new List<string>()
                    : new List<string>(refChain);

            // if type exists in chain return to prevent infinit loop
            if (refChain.Contains(asset.UID))
                return;
            refChain.Add(asset.UID);

            // recursivly process all parsed assets
            // starting from the botton of the ref tree.
            foreach (var refAsset in GetReferencedAssets(asset))
            {
                await ProcessAssetAsync(refAsset, null, foundBy, refChain);
            }

            var record = await _assetRepository.FindRecordAsync(asset);
            if (record == null)
            {
                record = new AssetRecord(asset, foundBy);
            }

            record = await _assetRepository.MergeCurrentRecordWithDBRecord(record, asset);

            record.UpdateTags(tags);

            var owningProgram = _programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(record.Asset)));
            record.SetOwningProgram(owningProgram);

            foreach (var rule in _notificationRules.Where(rule => (record.InScope || rule.CheckOutOfScope) && rule.Check(record)))
            {
                PwnInfraContext.NotificationSender.Send(record.Asset, rule);
            }

            var matchingTasks = _taskDefinitions.Where(def => ((def.MatchOutOfScope && def.Matches(record)) 
                                                            || (record.InScope && record.OwningProgram.Policy.Allows(def)
                                                              ) && def.Matches(record)));

            foreach (var definition in matchingTasks)
            {
                // only queue tasks once per definition/asset pair
                var task = _assetRepository.FindTaskEntry(record.Asset, definition);
                if (task != null)
                    continue;

                record.Tasks.Add(new TaskEntry(definition, record));
            }

            await _assetRepository.SaveAsync(record);
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
