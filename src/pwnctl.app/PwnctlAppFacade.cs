using pwnctl.infra.Configuration;
using pwnctl.infra.Repositories;
using pwnctl.infra.Persistence;
using pwnctl.core;

namespace pwnctl.app
{
    public static class PwnctlAppFacade
    {
        public static void Setup()
        {
            ConfigurationManager.Load();
            PwnctlCoreShim.PublicSuffixRepository = CachedPublicSuffixRepository.Singleton;
            DatabaseInitializer.Initialize();
        }
    }    
}