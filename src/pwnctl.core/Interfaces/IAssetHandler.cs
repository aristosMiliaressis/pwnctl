using System;
using System.Threading.Tasks;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Interfaces
{
    public interface IAssetHandler
    {
        Task<BaseAsset> HandleAsync(BaseAsset asset);
    }
}