using System;
using System.Threading.Tasks;
using pwnctl.core.BaseClasses;
using pwnctl.core.Interfaces;
using pwnctl.app.Repositories;
using pwnctl.infra.Persistence;

namespace pwnctl.app.Handlers
{
    public abstract class BaseAssetHandler<TAsset> : IAssetHandler where TAsset : BaseAsset
    {
        protected IAssetRepository _repository = new AssetRepository();
        protected PwnctlDbContext _context = new();

        public async Task<BaseAsset> HandleAsync(BaseAsset asset)
        {
            return await HandleAsync((TAsset) asset);
        }

        protected abstract Task<TAsset> HandleAsync(TAsset asset);
    }
}