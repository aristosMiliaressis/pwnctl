using pwnwrk.domain.Assets.BaseClasses;

namespace pwnwrk.domain.Assets.Interfaces
{
    public interface AssetRepository
    {
        Task<Asset> LoadRelatedAssets(Asset asset);
        Task SaveAsync(Asset asset);
    }
}