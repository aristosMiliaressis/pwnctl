﻿namespace pwnctl.app.Assets;

using System.Threading.Tasks;
using pwnctl.domain.BaseClasses;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Queueing.DTO;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Tagging;
using pwnctl.app.Operations.Enums;
using pwnctl.app.Notifications.Enums;
using pwnctl.kernel;
using pwnctl.kernel.BaseClasses;

public sealed class AssetProcessor
{
    public async Task<bool> TryProcessAsync(string assetText, int taskId)
    {
        try
        {
            (bool result, string error) = await ProcessAsync(assetText, taskId);
            if (!result)
                PwnInfraContext.Logger.Error(error);
            
            return result;
        }
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Exception(ex);
            return false;
        }
    }

    public async Task<(bool, string)> ProcessAsync(string assetText, int taskId)
    {
        Result<AssetDTO, string> dto = TagParser.Parse(assetText);
        if (dto.Failed)
            return (false, dto.Error);

        Result<Asset, string> asset = AssetParser.Parse(dto.Value.Asset);
        if (asset.Failed)
            return (false, asset.Error);

        await ProcessAssetAsync(asset.Value, dto.Value.Tags, taskId);

        return (true, "");
    }

    internal async Task ProcessAssetAsync(Asset asset, Dictionary<string, string>? tags, int taskId, List<Asset>? refChain = null)
    {
        refChain = refChain is null
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

        var foundByTask = await PwnInfraContext.TaskRepository.FindAsync(taskId);

        var record = await PwnInfraContext.AssetRepository.FindRecordAsync(asset);
        if (record is null)
        {
            record = new AssetRecord(asset, foundByTask);
        }

        var oldTags = record.Tags.ToDictionary(t => t.Name, t => t.Value);
        record.MergeTags(tags, updateExisting: foundByTask.Operation.Type == OperationType.Monitor);

        bool scopeChanged = false;
        if (!record.InScope)
        {
            record = await PwnInfraContext.AssetRepository.UpdateRecordReferences(record, asset);

            var scope = foundByTask.Operation.Scope.Definitions.FirstOrDefault(scope => scope.Definition.Matches(record.Asset)
                                                                        || scope.Definition.Matches(asset));
            if (scope is not null)
            {
                scopeChanged = true;
                record.SetScopeId(scope.Definition.Id);
            }
        }

        if (scopeChanged || record.Id == default || record.Tags.Any(t => t.Id == default) || foundByTask.Operation.Type == OperationType.Monitor)
        {
            if (foundByTask.Definition.CheckNotificationRules)
            {
                await CheckNotificationRulesAsync(record);
            }

            if (foundByTask.Operation.Type == OperationType.Crawl)
            {
                GenerateCrawlingTasks(foundByTask.Operation, record);
            }
            else if (foundByTask.Operation.Type == OperationType.Monitor)
            {
                await CheckMonitoringRulesAsync(record, oldTags, foundByTask);
            }
        }

        IEnumerable<TaskRecord> newTasks = await PwnInfraContext.AssetRepository.SaveAsync(record);

        var allowedTasks = foundByTask.Operation.Policy.GetAllowedTasks();

        newTasks.ToList().ForEach(task => task.Definition = allowedTasks.First(t => t.Id == task.DefinitionId));

        newTasks = newTasks.Where(t => t.Definition.Profile.Phase <= foundByTask.Operation.CurrentPhase);

        await PwnInfraContext.TaskQueueService.EnqueueBatchAsync(newTasks.Where(t => t.Definition.ShortLived).Select(t => new ShortLivedTaskDTO(t)));

        await PwnInfraContext.TaskQueueService.EnqueueBatchAsync(newTasks.Where(t => !t.Definition.ShortLived).Select(t => new LongLivedTaskDTO(t)));
    }

    private void GenerateCrawlingTasks(Operation operation, AssetRecord record)
    {
        var allowedTasks = operation.Policy.GetAllowedTasks();

        foreach (var defGroup in allowedTasks.Where(t => t.Subject.Value == record.Asset.GetType().Name).GroupBy(t => t.Filter))
        {
            if (!record.InScope && !defGroup.Any(def => def.MatchOutOfScope))
                continue;

            if (!defGroup.First().Matches(record))
                continue;

            foreach (var def in defGroup)
            {
                if (!record.InScope && !def.MatchOutOfScope)
                    continue;

                // only queue tasks once per definition/asset pair
                var task = PwnInfraContext.TaskRepository.Find(record, def);
                if (task is not null)
                    continue;

                task = new TaskRecord(operation, def, record);
                record.Tasks.Add(task);
            }
        }
    }

    private async Task CheckNotificationRulesAsync(AssetRecord record)
    {
        if (_notificationRules == null)
            _notificationRules = PwnInfraContext.NotificationRepository.ListRules();

        await Parallel.ForEachAsync(_notificationRules, async (rule, token) =>
        {
            if ((record.InScope || rule.CheckOutOfScope) && rule.Check(record))
            {
                // only send notifications once
                var notification = await PwnInfraContext.NotificationRepository.FindNotificationAsync(record.Asset, rule);
                if (notification is not null)
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
        Notification? notification = null;

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

    private IEnumerable<Asset> GetReferencedAssets(Asset asset)
    {
        var assetProperties = asset.GetType().GetProperties();
        List<Asset> assets = assetProperties
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)))
                .Select(rf => (Asset?)rf.GetValue(asset))
                .Where(a => a is not null)
                .ToList();

        assets.AddRange(assetProperties
                .Where(p => p.PropertyType.IsGenericType
                        && p.PropertyType.GetGenericArguments()[0].IsAssignableTo(typeof(Asset))
                        && p.GetValue(asset) is not null)
                .SelectMany(rf => (IEnumerable<Asset>)(rf.GetValue(asset) ?? new List<Asset>())));

        return assets;
    }

    private IEnumerable<NotificationRule> _notificationRules;
}
