using System;
using System.Reflection;
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

            if (!context.NotificationRules.Any())
            {
                string[] seedResources = Assembly.GetEntryAssembly().GetManifestResourceNames();

                foreach (string resourceName in seedResources)
                {
                    if (resourceName.EndsWith(".td.yml"))
                    {
                        await SeedTaskProfileAsync(context, resourceName);
                    }
                    else if (resourceName.EndsWith(".nr.yml"))
                    {
                        await SeedNotificationRulesAsync(context, resourceName);
                    }
                }
            }
            if (!context.Users.Any() && userManger is not null)
            {
                await SeedAdminUser(userManger);
            }
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

        private static async Task SeedTaskProfileAsync(PwnctlDbContext context, string resourceName)
        {
            Stream yamlStream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName);
            StreamReader reader = new StreamReader(yamlStream);
            string yamlText = reader.ReadToEnd();

            TaskConfigFile file;
            try
            {
                file = _deserializer.Deserialize<TaskConfigFile>(yamlText);
            }
            catch (Exception ex)
            {
                throw new ConfigValidationException(resourceName, $"Deserialization of {resourceName} failed", ex);
            }

            var passed = ConfigValidator.TryValidateTaskDefinitions(file, out string? errorMessage);
            if (!passed)
            {
                throw new ConfigValidationException(resourceName, errorMessage);
            }

            var definitions = file.TaskDefinitions.Select(d => d.ToEntity()).ToList();

            var profile = context.TaskProfiles.FirstOrDefault(p => p.Name == ShortName.Create(file.Profile));
            if (profile is not null)
            {
                return;
            }

            profile = new TaskProfile(file.Profile, file.Phase, definitions);
            context.Add(profile);

            await context.SaveChangesAsync();
        }

        private static async Task SeedNotificationRulesAsync(PwnctlDbContext context, string resourceName)
        {
            Stream yamlStream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName);
            StreamReader reader = new StreamReader(yamlStream);
            string yamlText = reader.ReadToEnd();

            List<NotificationRuleDTO> notificationRules;
            try
            {
                notificationRules = _deserializer.Deserialize<List<NotificationRuleDTO>>(yamlText);
            }
            catch (Exception ex)
            {
                throw new ConfigValidationException(resourceName, $"Deserialization of {resourceName} failed", ex);
            }

            var passed = ConfigValidator.TryValidateNotificationRules(notificationRules, out string? errorMessage);
            if (!passed)
            {
                throw new ConfigValidationException(resourceName, errorMessage);
            }

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
