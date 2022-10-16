using pwnwrk.infra.Configuration;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Repositories;
using pwnwrk.infra.Persistence;
using pwnwrk.domain;
using Serilog.Core;

namespace pwnwrk.infra
{
    public static class PwnContext
    {
        static PwnContext()
        {
            PwnContext.Config = PwnConfigFactory.Create();
            PwnContext.Logger = PwnLoggerFactory.Create();
            PwnwrkDomainShim.PublicSuffixRepository = PublicSuffixRepository.Singleton;
            DatabaseInitializer.InitializeAsync().Wait();
        }

        public static AppConfig Config { get; set; }
        public static Logger Logger { get; set; }
    }
}