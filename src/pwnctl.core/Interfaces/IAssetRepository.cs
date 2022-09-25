using pwnctl.core.BaseClasses;

namespace pwnctl.core.Interfaces
{
    public interface IAssetRepository
    {
        Task UpdateAsync(BaseAsset asset);
        Task AddOrUpdateAsync(BaseAsset asset);
        bool CheckIfExists(BaseAsset asset);
    }
}