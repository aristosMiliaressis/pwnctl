using pwnwrk.domain.Assets.BaseClasses;

namespace pwnwrk.infra.Repositories
{
    public interface IAssetRepository
    {
        Task SaveAsync(Asset asset);
    }
}