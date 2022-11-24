using pwnwrk.domain.Assets.BaseClasses;

namespace pwnwrk.infra.Repositories
{
    public interface IAssetRepository
    {
        Task<Asset> LoadRelatedAssets(Asset asset);
        Task SaveAsync(Asset asset);
    }
}