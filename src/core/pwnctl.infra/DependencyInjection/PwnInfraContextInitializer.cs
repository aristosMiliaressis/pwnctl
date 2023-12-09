using System;

namespace pwnctl.infra.DependencyInjection;

using pwnctl.domain.Interfaces;
using pwnctl.app;
using pwnctl.infra.Configuration;
using pwnctl.infra.Repositories;
using pwnctl.infra.Scheduling;
using pwnctl.infra.Serialization;
using pwnctl.infra.Logging;
using pwnctl.infra.Persistence;

public static class PwnInfraContextInitializer
{
    public static void Setup()
    {
        try 
        {
            var config = PwnConfigFactory.Create();
            var logger = PwnLoggerFactory.Create(config);

            PublicSuffixRepository.Instance = new FsPublicSuffixRepository();

            var serializer = new AppJsonSerializer();
            var evaluator = new CSharpFilterEvaluator();

            var context = new PwnctlDbContext();
            var assetRepo = new AssetDbRepository(context);
            var taskRepo = new TaskDbRepository(context);
            var notificationRepo = new NotificationDbRepository(context);
            var eventBridgeClient = new EventBridgeClient();

            PwnInfraContext.Setup(config, logger, serializer, evaluator, assetRepo, taskRepo, notificationRepo, eventBridgeClient);
        }
        catch (Exception ex)
        {
            PwnLoggerFactory.DefaultLogger.Exception(ex);
            throw;
        }
    }

    public static void Register<TIface, TImpl>()
    {
        try 
        {
            var prop = typeof(PwnInfraContext).GetProperties().FirstOrDefault(p => p.PropertyType == typeof(TIface));
            if (prop is null)
                throw new Exception($"Property of type {typeof(TIface).Name} not found.");
            
            prop.SetValue(null, Activator.CreateInstance(typeof(TImpl)));
        }
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Exception(ex);
            throw;
        }
    }
}
