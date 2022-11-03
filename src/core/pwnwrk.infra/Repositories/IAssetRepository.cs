using pwnwrk.domain.Assets.BaseClasses;

namespace pwnwrk.infra.Repositories
{
    public interface IAssetRepository
    {
        Task UpdateAsync(Asset asset);
        Task AddOrUpdateAsync(Asset asset);
        bool CheckIfExists(Asset asset);
    }
}