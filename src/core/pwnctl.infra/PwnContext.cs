using pwnctl.domain.Interfaces;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Configuration;
using pwnctl.infra.Repositories;
using pwnctl.infra.Logging;
using pwnctl.infra.Serialization;
using pwnctl.infra.Notifications;
using Serilog;

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
    }
}