using pwnctl.domain.Interfaces;
using pwnctl.app.Interfaces;
using pwnctl.infra.Configuration;
using pwnctl.infra.Repositories;
using pwnctl.infra.Logging;
using pwnctl.infra.Serialization;
using pwnctl.infra.Notifications;

namespace pwnctl.infra
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