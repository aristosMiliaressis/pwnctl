using Microsoft.EntityFrameworkCore;
using pwnwrk.domain.Entities;
using pwnwrk.infra.Notifications;
using Microsoft.Extensions.FileSystemGlobbing;
using pwnwrk.infra.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace pwnwrk.infra.Persistence
{
    public static class DatabaseInitializer
    {
        public static async Task<PwnctlDbContext> InitializeAsync()
        {
            PwnctlDbContext instance = new();

            if (ConfigurationManager.Config.IsTestRun)
            {
                await instance.Database.EnsureDeletedAsync();
            }

            if (instance.Database.GetPendingMigrations().Any())
            {
                await instance.Database.MigrateAsync();
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            if (!instance.TaskDefinitions.Any())
            {
                var taskText = File.ReadAllText($"{AppConfig.InstallPath}/seed/task-definitions.yml");

                var taskDefinitions = deserializer.Deserialize<List<TaskDefinition>>(taskText);

                instance.TaskDefinitions.AddRange(taskDefinitions);
                await instance.SaveChangesAsync();
            }

            if (!instance.Programs.Any())
            {
                Matcher matcher = new();
                matcher.AddInclude("target-*.yml");

                foreach (string file in matcher.GetResultsInFullPath($"{AppConfig.InstallPath}/seed/"))
                {
                    var programText = File.ReadAllText(file);
                    var program = deserializer.Deserialize<Program>(programText);

                    instance.ScopeDefinitions.AddRange(program.Scope);
                    instance.OperationalPolicies.Add(program.Policy);
                    instance.Programs.Add(program);
                    await instance.SaveChangesAsync();
                }
            }

            if (!instance.NotificationRules.Any())
            {
                var taskText = File.ReadAllText($"{AppConfig.InstallPath}/seed/notification-rules.yml");
                var notificationSettings = deserializer.Deserialize<NotificationSettings>(taskText);

                instance.NotificationProviderSettings.AddRange(notificationSettings.Providers);
                instance.NotificationRules.AddRange(notificationSettings.Rules);
                await instance.SaveChangesAsync();
            }

            return instance;
        }
    }
}
