using pwnctl.core.BaseClasses;

namespace pwnctl.core.Interfaces
{
    public interface IAssetRepository
    {
        Task<BaseAsset> AddOrUpdateAsync(BaseAsset asset);
        bool CheckIfExists(BaseAsset asset);
    }
}