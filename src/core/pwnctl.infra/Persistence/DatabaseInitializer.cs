using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Data.Sqlite;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.infra.Configuration.Validation.Exceptions;
using pwnctl.infra.Configuration.Validation;
using pwnctl.infra.Configuration;
using pwnctl.app.Common.ValueObjects;
using pwnctl.app.Users.Entities;
using pwnctl.app.Users.Enums;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.AspNetCore.Identity;
using pwnctl.app;

namespace pwnctl.infra.Persistence
{
    public static class DatabaseInitializer
    {
        private static IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

        public static async Task InitializeAsync(UserManager<User>? userManger)
        {
            PwnctlDbContext context = new();

            if (EnvironmentVariables.USE_LOCAL_INTEGRATIONS)
            {
                await context.Database.EnsureDeletedAsync();
            }

            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            if (!context.Users.Any() && userManger is not null)
            {
                await SeedAdminUser(userManger);
            }
        }

        public static async Task SeedAsync()
        {
            PwnctlDbContext context = new();

            await SeedTaskDefinitionsAsync(context);

            await SeedNotificationRulesAsync(context);
        }

        private static async Task SeedAdminUser(UserManager<User> userManger)
        {
            var ssmClient = new AmazonSecretsManagerClient();
            var password = await ssmClient.GetSecretValueAsync(new GetSecretValueRequest
            {
                SecretId = "/aws/secret/pwnctl/admin_password"
            });

            var admin = new User
            {
                UserName = "admin",
                Role = UserRole.SuperUser
            };

            var result = await userManger.CreateAsync(admin, password.SecretString);
            if (!result.Succeeded)
            {
                PwnInfraContext.Logger.Error(string.Join("\n", result.Errors.Select(e => e.Description)));
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

                var passed = ConfigValidator.TryValidateTaskDefinitions(taskFile, out string? errorMessage);
                if (!passed)
                {
                    throw new ConfigValidationException(taskFile, errorMessage);
                }

                var taskText = File.ReadAllText(taskFile);
                var file = _deserializer.Deserialize<TaskConfigFile>(taskText);

                var definitions = file.TaskDefinitions.Select(d => d.ToEntity()).ToList();

                var profile = context.TaskProfiles.FirstOrDefault(p => p.Name == ShortName.Create(file.Profile));
                if (profile is not null)
                {
                    continue;
                }

                profile = new TaskProfile(file.Profile, file.Phase, definitions);
                context.Add(profile);

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

                var passed = ConfigValidator.TryValidateNotificationRules(notificationFile, out string? errorMessage);
                if (!passed)
                {
                    throw new ConfigValidationException(notificationFile, errorMessage);
                }

                var taskText = File.ReadAllText(notificationFile);
                var notificationRules = _deserializer.Deserialize<List<NotificationRuleDTO>>(taskText);

                foreach (var rule in notificationRules)
                {
                    var notificationRule = context.NotificationRules.FirstOrDefault(p => p.Name == ShortName.Create(rule.Name));
                    if (notificationRule is not null)
                    {
                        continue;
                    }

                    context.NotificationRules.Add(rule.ToEntity());
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
