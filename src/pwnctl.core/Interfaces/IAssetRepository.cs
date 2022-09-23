using pwnctl.core.BaseClasses;

namespace pwnctl.core.Interfaces
{
    public interface IAssetRepository
    {
        BaseAsset AddOrUpdate(BaseAsset asset);
        bool CheckIfExists(BaseAsset asset);
        BaseAsset GetMatchingAsset(BaseAsset asset);
    }
}