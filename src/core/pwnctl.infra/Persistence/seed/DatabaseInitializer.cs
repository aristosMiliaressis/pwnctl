using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.infra.Configuration.Validation.Exceptions;
using pwnctl.infra.Configuration.Validation;
using pwnctl.infra.Configuration;
using pwnctl.infra.Queueing;
using pwnctl.infra.Repositories;
using pwnctl.app.Common.ValueObjects;

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

            if (EnvironmentVariables.TEST_RUN && EnvironmentVariables.DELETE_DB)
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
        }

        private static async Task SeedTaskDefinitionsAsync(PwnctlDbContext context)
        {
            Matcher matcher = new();
            matcher.AddInclude("*.td.yml");

            foreach (string taskFile in matcher.GetResultsInFullPath(Path.Combine(EnvironmentVariables.INSTALL_PATH, "seed/")))
            {
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
                var file = _deserializer.Deserialize<TaskDefinitionFile>(taskText);

                foreach (var profileName in file.Profiles)
                {
                    var definitions = file.TaskDefinitions.Select(d => new TaskDefinition
                    {
                        Name = d.ShortName.Value,
                        Subject = d.SubjectClass.Value,
                        CommandTemplate = d.CommandTemplate,
                        IsActive = d.IsActive,
                        Aggressiveness = d.Aggressiveness,
                        Filter = d.Filter,
                        MatchOutOfScope = d.MatchOutOfScope
                    }).ToList();

                    var profile = context.TaskProfiles.FirstOrDefault(p => p.ShortName == ShortName.Create(profileName));
                    if (profile == null)
                    {
                        profile = new TaskProfile(profileName, definitions);
                        context.Add(profile);
                        continue;
                    }

                    profile.TaskDefinitions.AddRange(definitions);
                    context.Update(profile);
                }

                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedNotificationRulesAsync(PwnctlDbContext context)
        {
            Matcher matcher = new();
            matcher.AddInclude("*.nr.yml");

            foreach (string notificationFile in matcher.GetResultsInFullPath(Path.Combine(EnvironmentVariables.INSTALL_PATH, "seed/")))
            {
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
        }
    }
}
