
namespace pwnctl.infra.DependencyInjection;

using pwnctl.domain.Interfaces;
using pwnctl.app;
using pwnctl.infra.Configuration;
using pwnctl.infra.Notifications;
using pwnctl.infra.Repositories;
using pwnctl.infra.Serialization;
using pwnctl.infra.Logging;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Persistence;

public static class PwnInfraContextInitializer
{
    public static void Setup()
    {
        PublicSuffixRepository.Instance = new FsPublicSuffixRepository();
        CloudServiceRepository.Instance = new FsCloudServiceRepository();

        var config = PwnConfigFactory.Create();
        var sender = EnvironmentVariables.TEST_RUN
                    ? (NotificationSender)new MockNotificationSender()
                    : (NotificationSender)new DiscordNotificationSender();
        var logger = PwnLoggerFactory.Create(config, sender);
        var serializer = new AppJsonSerializer();
        var evaluator = new CSharpFilterEvaluator();

        PwnInfraContext.Setup(config, logger, serializer, evaluator, sender);

        if (EnvironmentVariables.TEST_RUN)
        {
            DatabaseInitializer.InitializeAsync().Wait();
        }
    }
}