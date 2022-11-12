using pwnwrk.domain.Assets.Interfaces;
using pwnwrk.infra.Configuration;
using pwnwrk.infra.Repositories;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Serialization;
using Serilog;

namespace pwnwrk.infra
{
    public static class PwnContext
    {
        static PwnContext()
        {
            IFilterEvaluator.Instance = new CSharpFilterEvaluator();
            IPublicSuffixRepository.Instance = new PublicSuffixRepository();
        }

        public static AppConfig Config { get; private set; } = PwnConfigFactory.Create();
        public static ILogger Logger { get; private set; } = PwnLoggerFactory.Create();
        public static ISerializer Serializer { get; private set; } = new AppJsonSerializer();
    }
}