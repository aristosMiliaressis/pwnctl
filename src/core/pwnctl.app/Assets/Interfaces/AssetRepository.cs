using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;

namespace pwnctl.app.Assets.Interfaces
{
    public interface AssetRepository
    {
        Task<AssetRecord> FindRecordAsync(Asset asset);
        Asset FindMatching(Asset asset);
        TaskEntry FindTaskEntry(Asset asset, TaskDefinition def);
        Notification FindNotification(Asset asset, NotificationRule rule);
        Task<AssetRecord> UpdateRecordReferences(AssetRecord record, Asset asset);
        Task SaveAsync(AssetRecord asset);

        Task<List<AssetRecord>> ListHostsAsync();
        Task<List<AssetRecord>> ListEndpointsAsync();
        Task<List<AssetRecord>> ListServicesAsync();
        Task<List<AssetRecord>> ListNetRangesAsync();
        Task<List<AssetRecord>> ListDomainsAsync();
        Task<List<AssetRecord>> ListDNSRecordsAsync();
        Task<List<AssetRecord>> ListEmailsAsync();
    }
}