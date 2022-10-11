using pwnwrk.infra.Configuration;
using pwnwrk.infra.Repositories;
using pwnwrk.infra.Persistence;
using pwnwrk.domain;
using System.Threading.Tasks;

namespace pwnwrk.infra
{
    public static class PwnwrkInfraFacade
    {
        public static async Task SetupAsync()
        {
            ConfigurationManager.Load();
            PwnwrkDomainShim.PublicSuffixRepository = PublicSuffixRepository.Singleton;
            await DatabaseInitializer.InitializeAsync();
        }
    }    
}