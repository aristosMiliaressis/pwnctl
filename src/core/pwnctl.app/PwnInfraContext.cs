using pwnctl.app.Common.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Logging.Interfaces;
using pwnctl.app.Configuration;

namespace pwnctl.app
{
    public static class PwnInfraContext
    {
        public static void Setup(AppConfig config, AppLogger logger, Serializer serializer, FilterEvaluator evaluator, NotificationSender sender)
        {
            Config = config;
            Logger = logger;
            Serializer = serializer;
            FilterEvaluator = evaluator;
            NotificationSender = sender;
        }

        public static AppConfig Config { get; set; }
        public static AppLogger Logger { get; private set; }
        public static Serializer Serializer { get; private set; }
        public static NotificationSender NotificationSender { get; private set; }
        public static FilterEvaluator FilterEvaluator { get; private set; }
    }
}