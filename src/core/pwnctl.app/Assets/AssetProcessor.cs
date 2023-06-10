using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Operations.Enums;
using pwnctl.app.Notifications.Enums;
using pwnctl.kernel;

namespace pwnctl.app.Assets
{
    public sealed class AssetProcessor
    {
        private readonly AssetRepository _assetRepository;
        private readonly TaskRepository _taskRepository;
        private readonly TaskQueueService _taskQueueService;
        private readonly List<NotificationRule> _notificationRules;
        private readonly List<TaskDefinition> _outOfScopeTasks;

        public AssetProcessor(AssetRepository assetRepo, TaskRepository taskRepo, TaskQueueService taskQueueService,
                            List<NotificationRule> rules, List<TaskDefinition> outOfScopeTasks)
        {
            _assetRepository = assetRepo;
            _taskRepository = taskRepo;
            _taskQueueService = taskQueueService;
            _outOfScopeTasks = outOfScopeTasks;
            _notificationRules = rules;
        }

        public async Task<bool> TryProcessAsync(string assetText, Operation operation, TaskEntry foundByTask = null)
        {
            try
            {
                await ProcessAsync(assetText, operation, foundByTask);
                return true;
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
                return false;
            }
        }

        public async Task ProcessAsync(string assetText, Operation operation, TaskEntry foundByTask)
        {
            AssetDTO dto = TagParser.Parse(assetText);

            Asset asset = AssetParser.Parse(dto.Asset);

            await ProcessAssetAsync(asset, dto.Tags, operation, foundByTask);
        }

        internal async Task ProcessAssetAsync(Asset asset, Dictionary<string, object> tags, Operation operation, TaskEntry foundByTask, List<Asset> refChain = null)
        {
            refChain = refChain == null
                    ? new List<Asset>()
                    : new List<Asset>(refChain);

            // if type exists in chain return to prevent infinit loop
            if (refChain.Contains(asset))
                return;
            refChain.Add(asset);

            // recursivly process all parsed assets
            // starting from the botton of the ref tree.
            foreach (var refAsset in GetReferencedAssets(asset))
            {
                await ProcessAssetAsync(refAsset, null, operation, foundByTask, refChain);
            }

            var record = await _assetRepository.FindRecordAsync(asset);
            if (record == null)
            {
                record = new AssetRecord(asset, foundByTask);
            }

            record = await _assetRepository.UpdateRecordReferences(record, asset);
            record.MergeTags(tags, updateExisting: operation.Type == OperationType.Monitor);

            var scope = operation.Scope.Definitions.FirstOrDefault(scope => scope.Definition.Matches(record.Asset)
                                                                         || scope.Definition.Matches(asset));
            if (scope != null)
                record.SetScopeId(scope.Definition.Id);

            if (record.Id == default || record.Tags.Any(t => t.Id == default) || operation.Type == OperationType.Monitor)
            {
                await CheckMisconfigRulesAsync(record);

                if (operation.Type == OperationType.Crawl && operation.State != OperationState.Stopped)
                {
                    GenerateCrawlingTasks(operation, record);
                }
                else if (operation.Type == OperationType.Monitor)
                {
                    CheckMonitoringRules(record, foundByTask);
                }
            }

            await _assetRepository.SaveAsync(record);

            var allowedTasks = operation.Policy.GetAllowedTasks();
            allowedTasks.AddRange(_outOfScopeTasks);

            foreach (var task in record.Tasks)
            {
                task.Definition = allowedTasks.First(t => t.Id == task.DefinitionId);
                await _taskQueueService.EnqueueAsync(new PendingTaskDTO(task));
            }
        }

        private void GenerateCrawlingTasks(Operation operation, AssetRecord record)
        {
            var allowedTasks = operation.Policy.GetAllowedTasks();
            allowedTasks.AddRange(_outOfScopeTasks);

            foreach (var definition in allowedTasks.Where(def => (record.InScope || def.MatchOutOfScope) && def.Matches(record)))
            {
                // only queue tasks once per definition/asset pair
                var task = _taskRepository.Find(record, definition);
                if (task != null)
                    continue;

                task = new TaskEntry(operation, definition, record);
                record.Tasks.Add(task);
            }
        }

        private async Task CheckMisconfigRulesAsync(AssetRecord record)
        {
            foreach (var rule in _notificationRules.Where(rule => (record.InScope || rule.CheckOutOfScope) && rule.Check(record)))
            {
                // only send notifications once
                var notification = await _assetRepository.FindNotificationAsync(record.Asset, rule);
                if (notification != null)
                    continue;

                notification = new Notification(record, rule);

                PwnInfraContext.NotificationSender.Send(notification);
                notification.SentAt = SystemTime.UtcNow();
                record.Notifications.Add(notification);
            }
        }

        private void CheckMonitoringRules(AssetRecord record, TaskEntry foundByTask)
        {
            Notification notification = null;

            var rules = foundByTask.Definition.MonitorRules;

            if (record.Id == default)
            {
                notification = new Notification(record, foundByTask);

                PwnInfraContext.NotificationSender.Send(notification.GetText(), NotificationTopic.Monitoring);
                notification.SentAt = SystemTime.UtcNow();
                record.Notifications.Add(notification);
                return;
            }

            if (string.IsNullOrEmpty(rules.NotificationTemplate))
                return;

            var args = new Dictionary<string, object>
            {
                { record.Asset.GetType().Name, record.Asset },
                { "oldTags", foundByTask.Record },
                { "newTags", record }
            };

            if (!PwnInfraContext.FilterEvaluator.Evaluate(rules.PostCondition, args))
                return;

            notification = new Notification(record, foundByTask);

            PwnInfraContext.NotificationSender.Send(notification.GetText(), NotificationTopic.Monitoring);
            notification.SentAt = SystemTime.UtcNow();
            record.Notifications.Add(notification);
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
