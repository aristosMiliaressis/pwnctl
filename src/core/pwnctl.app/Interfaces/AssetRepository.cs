using pwnctl.app.Aggregates;
using pwnctl.app.Entities;
using pwnctl.domain.BaseClasses;

namespace pwnctl.app.Interfaces
{
    public interface AssetRepository
    {
        Task<AssetRecord> LoadRelatedAssets(Asset asset);
        TaskRecord FindTaskRecord(Asset asset, TaskDefinition def);
        Task SaveAsync(AssetRecord asset);
        Task SaveAsync(Asset asset);
    }
}