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

    Task<IEnumerable<AssetRecord>> ListNetworkHostsAsync(int pageIdx, CancellationToken token = default);
    Task<IEnumerable<AssetRecord>> ListHttpEndpointsAsync(int pageIdx, CancellationToken token = default);
    Task<IEnumerable<AssetRecord>> ListNetworkSocketsAsync(int pageIdx, CancellationToken token = default);
    Task<IEnumerable<AssetRecord>> ListNetworkRangesAsync(int pageIdx, CancellationToken token = default);
    Task<IEnumerable<AssetRecord>> ListDomainNamesAsync(int pageIdx, CancellationToken token = default);
    Task<IEnumerable<AssetRecord>> ListDomainNameRecordsAsync(int pageIdx, CancellationToken token = default);
    Task<IEnumerable<AssetRecord>> ListEmailsAsync(int pageIdx, CancellationToken token = default);
    Task<IEnumerable<AssetRecord>> ListVirtualHostsAsync(int pageIdx, CancellationToken token = default);
}