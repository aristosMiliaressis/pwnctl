using Microsoft.EntityFrameworkCore;
using pwnctl.core.Entities;
using pwnctl.infra.Notifications;
using Newtonsoft.Json;
using Microsoft.Extensions.FileSystemGlobbing;
using pwnctl.infra.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace pwnctl.infra.Persistence
{
    public static class DatabaseInitializer
    {
        public static PwnctlDbContext Initialize()
        {
            PwnctlDbContext instance = new();

            if (ConfigurationManager.Config.IsTestRun)
            {
                instance.Database.EnsureDeleted();
            }

            if (instance.Database.GetPendingMigrations().Any())
            {
                instance.Database.Migrate();
            }

            var taskDefinitionFile = $"{AppConfig.InstallPath}/seed/task-definitions.yml";

            if (!instance.TaskDefinitions.Any() && File.Exists(taskDefinitionFile))
            {
                var taskText = File.ReadAllText(taskDefinitionFile);
                var deserializer = new DeserializerBuilder()
                           .WithNamingConvention(PascalCaseNamingConvention.Instance)
                           .Build();
                var taskDefinitions = deserializer.Deserialize<List<TaskDefinition>>(taskText);

                instance.TaskDefinitions.AddRange(taskDefinitions);
                instance.SaveChanges();
            }

            if (!instance.Programs.Any())
            {
                Matcher matcher = new();
                matcher.AddInclude("target-*.json");

                foreach (string file in matcher.GetResultsInFullPath($"{AppConfig.InstallPath}/seed/"))
                {
                    var program = JsonConvert.DeserializeObject<Program>(File.ReadAllText(file));
                    instance.ScopeDefinitions.AddRange(program.Scope);
                    instance.OperationalPolicies.Add(program.Policy);
                    instance.Programs.Add(program);
                    instance.SaveChanges();
                }
            }

            var notificationRulesFile = $"{AppConfig.InstallPath}/seed/notification-rules.yml";

            if (!instance.NotificationRules.Any())
            {
                var taskText = File.ReadAllText(notificationRulesFile);
                var deserializer = new DeserializerBuilder()
                           .WithNamingConvention(PascalCaseNamingConvention.Instance)
                           .Build();
                var notificationSettings = deserializer.Deserialize<NotificationSettings>(taskText);

                instance.NotificationProviderSettings.AddRange(notificationSettings.Providers);
                instance.NotificationRules.AddRange(notificationSettings.Rules);
                instance.SaveChanges();
            }

            return instance;
        }
    }
}
