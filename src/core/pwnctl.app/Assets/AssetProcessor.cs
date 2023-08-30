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
using pwnctl.app.Notifications.Interfaces;
using System.Collections.Concurrent;

namespace pwnctl.app.Assets
{
    public sealed class AssetProcessor
    {
        private readonly AssetRepository _assetRepository;
        private readonly TaskRepository _taskRepository;
        private readonly NotificationRepository _notificationRepository;
        private readonly TaskQueueService _taskQueueService;
        private readonly List<NotificationRule> _notificationRules;
        private readonly List<TaskDefinition> _outOfScopeTasks;

        public AssetProcessor(AssetRepository assetRepo, TaskQueueService taskQueueService, TaskRepository taskRepo, NotificationRepository notificRepo)
        {
            _assetRepository = assetRepo;
            _taskRepository = taskRepo;
            _notificationRepository = notificRepo;
            _taskQueueService = taskQueueService;
            _outOfScopeTasks = taskRepo.ListOutOfScope();
            _notificationRules = notificRepo.ListRules();
        }

        public async Task<bool> TryProcessAsync(string assetText, int taskId)
        {
            try
            {
                await ProcessAsync(assetText, taskId);
                return true;
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
                return false;
            }
        }

        public async Task ProcessAsync(string assetText, int taskId)
        {
            AssetDTO dto = TagParser.Parse(assetText);

            Asset asset = AssetParser.Parse(dto.Asset);

            await ProcessAssetAsync(asset, dto.Tags, taskId);
        }

        internal async Task ProcessAssetAsync(Asset asset, Dictionary<string, object> tags, int taskId, List<Asset> refChain = null)
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
                await ProcessAssetAsync(refAsset, null, taskId, refChain);
            }

            var foundByTask = await _taskRepository.FindAsync(taskId);

            var record = await _assetRepository.FindRecordAsync(asset);
            if (record == null)
            {
                record = new AssetRecord(asset, foundByTask);
            }

            var oldTags = record.Tags.ToDictionary(t => t.Name, t => t.Value);
            record.MergeTags(tags, updateExisting: foundByTask.Operation.Type == OperationType.Monitor);

            if (!record.InScope)
            {
                record = await _assetRepository.UpdateRecordReferences(record, asset);

                var scope = foundByTask.Operation.Scope.Definitions.FirstOrDefault(scope => scope.Definition.Matches(record.Asset)
                                                                            || scope.Definition.Matches(asset));
                if (scope != null)
                    record.SetScopeId(scope.Definition.Id);
            }

            if (record.Id == default || record.Tags.Any(t => t.Id == default) || foundByTask.Operation.Type == OperationType.Monitor)
            {
                if (foundByTask.Definition.CheckNotificationRules)
                {
                    await CheckFindingRulesAsync(record);
                }

                if (foundByTask.Operation.Type == OperationType.Crawl
                 && foundByTask.Operation.State != OperationState.Stopped)
                {
                    GenerateCrawlingTasks(foundByTask.Operation, record);
                }
                else if (foundByTask.Operation.Type == OperationType.Monitor)
                {
                    await CheckMonitoringRulesAsync(record, oldTags, foundByTask);
                }
            }

            await _assetRepository.SaveAsync(record);

            var allowedTasks = foundByTask.Operation.Policy.GetAllowedTasks();
            allowedTasks.AddRange(_outOfScopeTasks);

            await Parallel.ForEachAsync(record.Tasks, async (task, token) =>
            {
                task.Definition = allowedTasks.First(t => t.Id == task.DefinitionId);
                await _taskQueueService.EnqueueAsync(new PendingTaskDTO(task));
            });
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

                task = new TaskRecord(operation, definition, record);
                record.Tasks.Add(task);
            }
        }

        private async Task CheckFindingRulesAsync(AssetRecord record)
        {
            await Parallel.ForEachAsync(_notificationRules, async (rule, token) =>
            {
                if ((record.InScope || rule.CheckOutOfScope) && rule.Check(record))
                {
                     // only send notifications once
                    var notification = await _assetRepository.FindNotificationAsync(record.Asset, rule);
                    if (notification != null)
                        return;

                    notification = new Notification(record, rule);

                    await PwnInfraContext.NotificationSender.SendAsync(notification);
                    notification.SentAt = SystemTime.UtcNow();
                    record.Notifications.Add(notification);
                }
            });
        }

        private async Task CheckMonitoringRulesAsync(AssetRecord record, Dictionary<string, string> oldTags, TaskRecord foundByTask)
        {
            Notification notification = null;

            var rules = foundByTask.Definition.MonitorRules;

            if (record.Id == default)
            {
                notification = new Notification(record, foundByTask);

                await PwnInfraContext.NotificationSender.SendAsync(notification.GetText(), NotificationTopic.Monitoring);
                notification.SentAt = SystemTime.UtcNow();
                record.Notifications.Add(notification);
                return;
            }

            if (string.IsNullOrEmpty(rules.NotificationTemplate))
                return;

            var args = new Dictionary<string, object>
            {
                { record.Asset.GetType().Name, record.Asset },
                { "oldTags", oldTags },
                { "newTags", record }
            };

            if (!PwnInfraContext.FilterEvaluator.Evaluate(rules.PostCondition, args))
                return;

            notification = new Notification(record, foundByTask);

            await PwnInfraContext.NotificationSender.SendAsync(notification.GetText(), NotificationTopic.Monitoring);
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
