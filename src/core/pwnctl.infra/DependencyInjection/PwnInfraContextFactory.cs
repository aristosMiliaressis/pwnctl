
namespace pwnctl.infra.DependencyInjection;

using pwnctl.domain.Services;
using pwnctl.app;
using pwnctl.infra.Configuration;
using pwnctl.infra.Notifications;
using pwnctl.infra.Repositories;
using pwnctl.infra.Serialization;
using pwnctl.app.Logging;
using pwnctl.infra.Logging;

public static class PwnInfraContextInitializer
{
    public static void Setup()
    {
        PublicSuffixListService.Instance = new FsPublicSuffixListService();

        var config = PwnConfigFactory.Create();
        var sender = new DiscordNotificationSender();
        var logger = PwnLoggerFactory.Create(config, sender);
        var serializer = new AppJsonSerializer();
        var evaluator = new CSharpFilterEvaluator();

        PwnInfraContext.Setup(config, logger, serializer, evaluator, sender);
    }
}