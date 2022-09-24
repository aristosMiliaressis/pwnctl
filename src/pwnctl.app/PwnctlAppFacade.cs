using pwnctl.infra.Configuration;
using pwnctl.infra.Repositories;
using pwnctl.infra.Persistence;
using pwnctl.core;

namespace pwnctl.app
{
    public static class PwnctlAppFacade
    {
        public static async Task SetupAsync()
        {
            ConfigurationManager.Load();
            PwnctlCoreShim.PublicSuffixRepository = CachedPublicSuffixRepository.Singleton;
            await DatabaseInitializer.InitializeAsync();
        }
    }    
}