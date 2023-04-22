﻿using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Tasks.Interfaces;

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

            // The desired traversal of the following reference sub-tree is (B-C-A-B-C)
            // the current solution results in the traversal (B-C-A-B-C-A) which is suboptimal
            //    A
            //   / \
            //  B   C
            // TODO: optimize & decuple the reference graph traversal, from the asset processing
            await ProcessAssetAsync(asset, dto.Tags, operation, foundByTask);
            await ProcessAssetAsync(asset, dto.Tags, operation, foundByTask);
        }

        private async Task ProcessAssetAsync(Asset asset, Dictionary<string, object> tags, Operation operation, TaskEntry foundByTask, List<Asset> refChain = null)
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
            record.UpdateTags(tags);

            if (operation != null)
            {
                var scope = operation.Scope.Definitions.FirstOrDefault(scope => scope.Definition.Matches(record.Asset));
                if (scope != null)
                    record.SetScope(scope.Definition);

                foreach (var rule in _notificationRules.Where(rule => (record.InScope || rule.CheckOutOfScope) && rule.Check(record)))
                {
                    // only send notifications once
                    var notification = await _assetRepository.FindNotificationAsync(record.Asset, rule);
                    if (notification != null)
                        continue;
                    
                    notification = new Notification(record, rule);

                    PwnInfraContext.NotificationSender.Send(notification);
                    notification.SentAt = DateTime.UtcNow;
                    record.Notifications.Add(notification);
                }

                var allowedTasks = operation.Policy.GetAllowedTasks();
                allowedTasks.AddRange(_outOfScopeTasks);

                foreach (var definition in allowedTasks.Where(def => def.Matches(record)))
                {
                    // only queue tasks once per definition/asset pair
                    var task = _taskRepository.Find(record, definition);
                    if (task != null)
                        continue;

                    task = new TaskEntry(operation, definition, record);
                    await _taskRepository.AddAsync(task);

                    await _taskQueueService.EnqueueAsync(new PendingTaskDTO(task));

                    task.Queued();
                    record.Tasks.Add(task);
                }
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
