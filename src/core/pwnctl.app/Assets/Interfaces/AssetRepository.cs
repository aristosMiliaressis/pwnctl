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
    Task SaveAsync(AssetRecord record);

    Task<List<AssetRecord>> ListInScopeAsync(int scopeId, AssetClass[] assetClasses, int pageIdx, CancellationToken token = default);

    Task<List<AssetRecord>> ListHostsAsync(int pageIdx);
    Task<List<AssetRecord>> ListEndpointsAsync(int pageIdx);
    Task<List<AssetRecord>> ListServicesAsync(int pageIdx);
    Task<List<AssetRecord>> ListNetRangesAsync(int pageIdx);
    Task<List<AssetRecord>> ListDomainsAsync(int pageIdx);
    Task<List<AssetRecord>> ListDNSRecordsAsync(int pageIdx);
    Task<List<AssetRecord>> ListEmailsAsync(int pageIdx);
}