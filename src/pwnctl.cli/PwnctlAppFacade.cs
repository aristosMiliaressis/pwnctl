using pwnwrk.infra.Configuration;
using pwnwrk.infra.Repositories;
using pwnwrk.infra.Persistence;
using pwnwrk.domain;
using System.Threading.Tasks;

namespace pwnctl.cli
{
    public static class PwnctlAppFacade
    {
        public static async Task SetupAsync()
        {
            ConfigurationManager.Load();
            PwnwrkCoreShim.PublicSuffixRepository = PublicSuffixRepository.Singleton;
            await DatabaseInitializer.InitializeAsync();
        }
    }    
}