using pwnwrk.infra.Configuration;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Repositories;
using pwnwrk.infra.Persistence;
using pwnwrk.domain;

namespace pwnwrk.infra
{
    public static class PwnwrkInfraFacade
    {
        public static async Task SetupAsync()
        {
            PwnContext.Config = PwnConfigFactory.Create();
            PwnContext.Logger = PwnLoggerFactory.Create();
            PwnwrkDomainShim.PublicSuffixRepository = PublicSuffixRepository.Singleton;
            await DatabaseInitializer.InitializeAsync();
        }
    }    
}