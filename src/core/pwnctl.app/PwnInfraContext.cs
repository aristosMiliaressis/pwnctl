namespace pwnctl.app;

using pwnctl.app.Common.Interfaces;
using pwnctl.app.Logging.Interfaces;
using pwnctl.app.Configuration;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.app.Operations.Interfaces;

public static class PwnInfraContext
{
    public static void Setup(AppConfig config, AppLogger logger, Serializer serializer, FilterEvaluator evaluator, 
                            AssetRepository assetRepo, TaskRepository taskRepo, NotificationRepository notificationRepo,
                            OperationStateSubscriptionService opStateSubSrvs)
    {
        Config = config;
        Logger = logger;
        Serializer = serializer;
        FilterEvaluator = evaluator;
        AssetRepository = assetRepo;
        TaskRepository = taskRepo;
        NotificationRepository = notificationRepo;
        OperationStateSubscriptionService = opStateSubSrvs;
    }

    public static AppConfig Config { get; set; }
    public static AppLogger Logger { get; private set; }
    public static Serializer Serializer { get; private set; }
    public static NotificationSender NotificationSender { get; private set; }
    public static FilterEvaluator FilterEvaluator { get; private set; }
    public static CommandExecutor CommandExecutor { get; private set; }
    public static AssetRepository AssetRepository { get; private set; }
    public static TaskRepository TaskRepository { get; private set; }
    public static NotificationRepository NotificationRepository { get; private set; }
    public static TaskQueueService TaskQueueService { get; private set; }
    public static OperationStateSubscriptionService OperationStateSubscriptionService { get; private set; }
}