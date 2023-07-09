namespace pwnctl.infra.Configuration.Validation;

using System.Net;
using pwnctl.app;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.domain.Enums;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class ConfigValidator
{
    private static Dictionary<AssetClass, Asset> _sampleAssets = new Dictionary<AssetClass, Asset>
    {
        { AssetClass.Create(nameof(DomainName)), new DomainName("example.com") },
        { AssetClass.Create(nameof(HttpEndpoint)), new HttpEndpoint("http", new NetworkSocket(new DomainName("example.com"), 80), "/") },
        { AssetClass.Create(nameof(NetworkHost)), new NetworkHost(IPAddress.Parse("1.3.3.7")) },
        { AssetClass.Create(nameof(NetworkRange)), new NetworkRange(IPAddress.Parse("1.3.3.0"), 24) },
        { AssetClass.Create(nameof(NetworkSocket)), new NetworkSocket(new DomainName("example.com"), 443) },
        { AssetClass.Create(nameof(DomainNameRecord)), new DomainNameRecord(DnsRecordType.A, "example.com", "1.3.3.7") },
        { AssetClass.Create(nameof(Email)), new Email(new DomainName("example.com"), "mail@example.com") },
        { AssetClass.Create(nameof(HttpParameter)), new HttpParameter() },
    };

    private static IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();


    public static bool TryValidateTaskDefinitions(string fileName, out string errorMessage)
    {
        errorMessage = null;

        TaskConfigFile file;
        IEnumerable<TaskDefinition> taskDefinitions;
        try
        {
            var taskText = File.ReadAllText(fileName);
            file = _deserializer.Deserialize<TaskConfigFile>(taskText);
            taskDefinitions = file.TaskDefinitions.Select(d => d.ToEntity());
        }
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Error(ex.ToRecursiveExInfo());
            errorMessage = $"Deserialization of {fileName} failed";
            return false;
        }

        if (!file.Profiles.Any())
        {
            errorMessage = $"At least one profile is required";
            return false;
        }

        if (!taskDefinitions.Any())
        {
            errorMessage = $"At least one task definition is required";
            return false;
        }

        if (taskDefinitions.Select(d => d.Name.Value).Distinct().Count() != taskDefinitions.Count())
        {
            errorMessage = "Duplicate ShortName";
            return false;
        }

        foreach (var definition in taskDefinitions)
        {
            if (string.IsNullOrEmpty(definition.Name.Value))
            {
                errorMessage = "Null or Empty ShortName";
                return false;
            }

            if (definition.Subject == null || string.IsNullOrEmpty(definition.Subject.Value))
            {
                errorMessage = "Null or Empty Subject on " + definition.Name.Value;
                return false;
            }

            AssetRecord record;
            try
            {
                var asset = _sampleAssets[definition.Subject];
                record = new AssetRecord(asset);
                var op = new Operation();
                var taskRecord = new TaskRecord(op, definition, record);
                var test = taskRecord.Command;
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Error(ex.ToRecursiveExInfo());
                errorMessage = $"task definition {definition.Name.Value} failed to interpolate CommandTemplate arguments";
                return false;
            }

            try
            {
                definition.Matches(record);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Error(ex.ToRecursiveExInfo());
                errorMessage = $"task definition {definition.Name.Value} Filter through exception";
                return false;
            }
        }

        return true;
    }

    public static bool TryValidateNotificationRules(string fileName, out string errorMessage)
    {
        errorMessage = null;

        IEnumerable<NotificationRule> notificationRules;
        try
        {
            var ruleText = File.ReadAllText(fileName);
            var notificationRuleDTOs = _deserializer.Deserialize<List<NotificationRuleDTO>>(ruleText);
            notificationRules = notificationRuleDTOs.Select(r => r.ToEntity());
        }
        catch
        {
            errorMessage = $"Deserialization of {fileName} failed";
            return false;
        }

        if (notificationRules.Select(d => d.Name).Distinct().Count() != notificationRules.Count())
        {
            errorMessage = "Duplicate ShortName";
            return false;
        }

        foreach (var rule in notificationRules)
        {
            if (string.IsNullOrEmpty(rule.Name.Value))
            {
                errorMessage = "Null or Empty ShortName";
                return false;
            }

            if (rule.Subject == null || string.IsNullOrEmpty(rule.Subject.Value))
            {
                errorMessage = "Null or Empty Subject on " + rule.Name.Value;
                return false;
            }

            AssetRecord record = new AssetRecord(_sampleAssets[rule.Subject]);

            try
            {
                rule.Check(record);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Error(ex.ToRecursiveExInfo());
                errorMessage = $"notification rule {rule.Name.Value} Filter through exception";
                return false;
            }
        }

        return true;
    }
}
