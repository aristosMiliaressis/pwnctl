using pwnctl.infra.Repositories;
using pwnctl.infra.Persistence;
using pwnctl.core;

namespace pwnctl.app
{
    public static class PwnctlAppFacade
    {
        public static void Setup()
        {
            PwnctlDbContext.Initialize();
            PwnctlCoreShim.PublicSuffixRepository = CachedPublicSuffixRepository.Singleton;
        }
    }    
}