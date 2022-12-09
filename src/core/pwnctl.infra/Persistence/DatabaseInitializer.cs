using Microsoft.EntityFrameworkCore;
using pwnctl.app.Entities;
using Microsoft.Extensions.FileSystemGlobbing;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace pwnctl.infra.Persistence
{
    public static class DatabaseInitializer
    {
        public static async Task<PwnctlDbContext> InitializeAsync()
        {
            PwnctlDbContext instance = new();

            if (PwnContext.Config.IsTestRun)
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

            string taskFile = $"{PwnContext.Config.InstallPath}/seed/task-definitions.yml";
            if (!instance.TaskDefinitions.Any() && File.Exists(taskFile))
            {
                var taskText = File.ReadAllText(taskFile);
                var taskDefinitions = deserializer.Deserialize<List<TaskDefinition>>(taskText);

                instance.TaskDefinitions.AddRange(taskDefinitions);
                await instance.SaveChangesAsync();
            }

            if (!instance.Programs.Any())
            {
                Matcher matcher = new();
                matcher.AddInclude("target-*.yml");

                foreach (string file in matcher.GetResultsInFullPath($"{PwnContext.Config.InstallPath}/seed/"))
                {
                    var programText = File.ReadAllText(file);
                    var program = deserializer.Deserialize<Program>(programText);

                    instance.ScopeDefinitions.AddRange(program.Scope);
                    instance.OperationalPolicies.Add(program.Policy);
                    instance.Programs.Add(program);
                    await instance.SaveChangesAsync();
                }
            }

            string notificationFile = $"{PwnContext.Config.InstallPath}/seed/notification-rules.yml";
            if (!instance.NotificationRules.Any() && File.Exists(notificationFile))
            {
                var taskText = File.ReadAllText(notificationFile);
                var notificationRules = deserializer.Deserialize<List<NotificationRule>>(taskText);

                instance.NotificationRules.AddRange(notificationRules);
                await instance.SaveChangesAsync();
            }

            return instance;
        }
    }
}
