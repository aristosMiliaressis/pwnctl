namespace pwnctl.app.Assets.Interfaces;

using pwnctl.app.Assets.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.ValueObjects;

public interface AssetRepository
{
    Task<AssetRecord> FindRecordAsync(Asset asset);
    Task<AssetRecord> UpdateRecordReferences(AssetRecord record, Asset asset);
    Task<IEnumerable<TaskRecord>> SaveAsync(AssetRecord record);

    Task<List<AssetRecord>> ListInScopeAsync(int scopeId, AssetClass[] assetClasses, int pageIdx, CancellationToken token = default);

    Task<IEnumerable<AssetRecord>> ListHostsAsync(int pageIdx);
    Task<IEnumerable<AssetRecord>> ListEndpointsAsync(int pageIdx);
    Task<IEnumerable<AssetRecord>> ListServicesAsync(int pageIdx);
    Task<IEnumerable<AssetRecord>> ListNetRangesAsync(int pageIdx);
    Task<IEnumerable<AssetRecord>> ListDomainsAsync(int pageIdx);
    Task<IEnumerable<AssetRecord>> ListDNSRecordsAsync(int pageIdx);
    Task<IEnumerable<AssetRecord>> ListEmailsAsync(int pageIdx);
}