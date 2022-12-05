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
            FilterEvaluator.Instance = new CSharpFilterEvaluator();
            NotificationSender.Instance = new DiscordNotificationSender();
            PublicSuffixRepository.Instance = new PublicSuffixFileRepository();
            Serializer.Instance = new AppJsonSerializer();
        }

        public static AppConfig Config { get; private set; } = PwnConfigFactory.Create();
        public static ILogger Logger { get; private set; } = PwnLoggerFactory.Create();
        public static Serializer Serializer { get; private set; } = new AppJsonSerializer();
    }
}