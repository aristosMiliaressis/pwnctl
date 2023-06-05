using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;

namespace pwnctl.app.Assets.Interfaces
{
    public interface AssetRepository
    {
        Task<AssetRecord> FindRecordAsync(Asset asset);
        Task<Notification> FindNotificationAsync(Asset asset, NotificationRule rule);
        Task<AssetRecord> UpdateRecordReferences(AssetRecord record, Asset asset);
        Task SaveAsync(AssetRecord record);

        Task<List<AssetRecord>> ListInScopeAsync(int scopeId, CancellationToken token = default);

        Task<List<AssetRecord>> ListHostsAsync(int pageIdx, int pageSize = 4096);
        Task<List<AssetRecord>> ListEndpointsAsync(int pageIdx, int pageSize = 4096);
        Task<List<AssetRecord>> ListServicesAsync(int pageIdx, int pageSize = 4096);
        Task<List<AssetRecord>> ListNetRangesAsync(int pageIdx, int pageSize = 4096);
        Task<List<AssetRecord>> ListDomainsAsync(int pageIdx, int pageSize = 4096);
        Task<List<AssetRecord>> ListDNSRecordsAsync(int pageIdx, int pageSize = 4096);
        Task<List<AssetRecord>> ListEmailsAsync(int pageIdx, int pageSize = 4096);
    }
}
