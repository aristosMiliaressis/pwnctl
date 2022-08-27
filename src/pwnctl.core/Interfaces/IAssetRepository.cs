using pwnctl.core.BaseClasses;

namespace pwnctl.core.Interfaces
{
    public interface IAssetRepository
    {
        Task<BaseAsset> AddAsync(BaseAsset asset);
        bool CheckIfExists(BaseAsset asset);
        BaseAsset GetAsset(BaseAsset asset);
    }
}