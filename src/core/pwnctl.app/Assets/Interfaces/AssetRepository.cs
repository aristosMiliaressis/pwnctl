using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;

namespace pwnctl.app.Assets.Interfaces
{
    public interface AssetRepository
    {
        Task<AssetRecord> GetAssetRecord(Asset asset);
        TaskRecord FindTaskRecord(Asset asset, TaskDefinition def);
        Task SaveAsync(AssetRecord asset);
        Task SaveAsync(Asset asset);
    }
}