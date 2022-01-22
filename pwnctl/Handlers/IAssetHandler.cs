using System;
using System.Threading.Tasks;
using pwnctl.Entities;

namespace pwnctl.Handlers
{
    public interface IAssetHandler
    {
        Task HandleAsync(IAsset asset);
    }

    public interface IAssetHandler<IAsset> : IAssetHandler
    {
        
    }
}