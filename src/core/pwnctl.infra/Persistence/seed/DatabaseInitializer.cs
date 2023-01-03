using Microsoft.EntityFrameworkCore;
using pwnctl.app;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Notifications.Entities;
using Microsoft.Extensions.FileSystemGlobbing;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using pwnctl.app.Tasks.Validation.Exceptions;
using pwnctl.app.Tasks.Validation;

namespace pwnctl.infra.Persistence
{
    public static class DatabaseInitializer
    {
        private static IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();
        
        public static async Task InitializeAsync()
        {
            PwnctlDbContext context = new();

            if (PwnInfraContext.Config.IsTestRun)
            {
                await context.Database.EnsureDeletedAsync();
            }

            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            if (!context.TaskDefinitions.Any())
            {
                await SeedTaskDefinitionsAsync(context);
            }

            if (!context.NotificationRules.Any())
            {
                await SeedNotificationRulesAsync(context);
            }

            if (!context.Programs.Any())
            {
                await SeedTargetsAsync(context);
            }
        }

        private static async Task SeedTaskDefinitionsAsync(PwnctlDbContext context)
        {
            string taskFile = $"{PwnInfraContext.Config.InstallPath}/seed/task-definitions.yml";
            if (!File.Exists(taskFile))
            {
                throw new ConfigValidationException(taskFile, "File not found");
            }

            var passed = ConfigValidator.TryValidateTaskDefinitions(taskFile, out string errorMessage);
            if (!passed)
            {
                throw new ConfigValidationException(taskFile, errorMessage);
            }

            var taskText = File.ReadAllText(taskFile);
            var taskDefinitions = _deserializer.Deserialize<List<TaskDefinition>>(taskText);

            context.TaskDefinitions.AddRange(taskDefinitions);
            await context.SaveChangesAsync();
        }

        private static async Task SeedNotificationRulesAsync(PwnctlDbContext context)
        {
            string notificationFile = $"{PwnInfraContext.Config.InstallPath}/seed/notification-rules.yml";
            if (!File.Exists(notificationFile))
            {
                throw new ConfigValidationException(notificationFile, "File not found");
            }

            var passed = ConfigValidator.TryValidateNotificationRules(notificationFile, out string errorMessage);
            if (!passed)
            {
                throw new ConfigValidationException(notificationFile, errorMessage);
            }

            var taskText = File.ReadAllText(notificationFile);
            var notificationRules = _deserializer.Deserialize<List<NotificationRule>>(taskText);

            context.NotificationRules.AddRange(notificationRules);
            await context.SaveChangesAsync();
        }
    
        private static async Task SeedTargetsAsync(PwnctlDbContext context)
        {
            Matcher matcher = new();
            matcher.AddInclude("target-*.yml");

            foreach (string file in matcher.GetResultsInFullPath($"{PwnInfraContext.Config.InstallPath}/seed/"))
            {
                var programText = File.ReadAllText(file);
                var program = _deserializer.Deserialize<Program>(programText);

                context.ScopeDefinitions.AddRange(program.Scope);
                context.OperationalPolicies.Add(program.Policy);
                context.Programs.Add(program);
                await context.SaveChangesAsync();
            }
        }
    }
}
