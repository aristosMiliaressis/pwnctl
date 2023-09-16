using System;

namespace pwnctl.infra.DependencyInjection;

using pwnctl.domain.Interfaces;
using pwnctl.app;
using pwnctl.infra.Commands;
using pwnctl.infra.Configuration;
using pwnctl.infra.Notifications;
using pwnctl.infra.Queueing;
using pwnctl.app.Queueing.Interfaces;
using pwnctl.infra.Repositories;
using pwnctl.infra.Serialization;
using pwnctl.infra.Logging;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.infra.Persistence;
using Microsoft.AspNetCore.Identity;
using pwnctl.app.Common.Interfaces;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.app.Users.Entities;

public static class PwnInfraContextInitializer
{
    public static void Setup()
    {
        var config = PwnConfigFactory.Create();
        var logger = PwnLoggerFactory.Create(config);

        try 
        {
            PublicSuffixRepository.Instance = new FsPublicSuffixRepository();
            CloudServiceRepository.Instance = new FsCloudServiceRepository();

            var serializer = new AppJsonSerializer();
            var evaluator = new CSharpFilterEvaluator();

            var context = new PwnctlDbContext();
            var assetRepo = new AssetDbRepository(context);
            var taskRepo = new TaskDbRepository(context);
            var notificationRepo = new NotificationDbRepository(context);

            PwnInfraContext.Setup(config, logger, serializer, evaluator, assetRepo, taskRepo, notificationRepo);
        }
        catch (Exception ex)
        {
            logger.Exception(ex);
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
