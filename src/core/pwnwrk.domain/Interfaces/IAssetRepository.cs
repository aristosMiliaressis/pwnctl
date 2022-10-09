using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Interfaces
{
    public interface IAssetRepository
    {
        Task UpdateAsync(BaseAsset asset);
        Task AddOrUpdateAsync(BaseAsset asset);
        bool CheckIfExists(BaseAsset asset);
    }
}