
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
using Microsoft.AspNetCore.Identity;
using pwnctl.app.Users.Entities;

public static class PwnInfraContextInitializer
{
    public static async Task SetupAsync(UserManager<User> userManger = null)
    {
        PublicSuffixRepository.Instance = new FsPublicSuffixRepository();
        CloudServiceRepository.Instance = new FsCloudServiceRepository();

        var config = PwnConfigFactory.Create();
        var sender = EnvironmentVariables.TEST_RUN
                    ? (NotificationSender)new StubNotificationSender()
                    : (NotificationSender)new DiscordNotificationSender();

        var logger = PwnLoggerFactory.Create(config);
        var serializer = new AppJsonSerializer();
        var evaluator = new CSharpFilterEvaluator();

        PwnInfraContext.Setup(config, logger, serializer, evaluator, sender);

        if (EnvironmentVariables.TEST_RUN || EnvironmentVariables.IS_LAMBDA)
        {
            await DatabaseInitializer.InitializeAsync(userManger);
        }
    }
}
