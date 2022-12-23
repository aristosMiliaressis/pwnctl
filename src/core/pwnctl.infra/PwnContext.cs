using pwnctl.app.Common.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Configuration;
using pwnctl.infra.Repositories;
using pwnctl.infra.Logging;
using pwnctl.infra.Serialization;
using pwnctl.infra.Notifications;
using Serilog;
using pwnctl.domain.Services;

namespace pwnctl.infra
{
    public static class PwnContext
    {
        static PwnContext()
        {
            Setup();
        }

        public static void Setup()
        {
            FilterEvaluator.Instance = new CSharpFilterEvaluator();
            NotificationSender.Instance = new DiscordNotificationSender();
            PublicSuffixListService.Instance = new FsPublicSuffixListService();
            Serializer.Instance = new AppJsonSerializer();
        }

        public static AppConfig Config { get; set; } = PwnConfigFactory.Create();
        public static ILogger Logger { get; private set; } = PwnLoggerFactory.Create();
    }
}