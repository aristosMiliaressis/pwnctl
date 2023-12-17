using System.IO;
namespace pwnctl.infra.Configuration.Validation;

using System.Net;
using pwnctl.app;
using pwnctl.app.Assets.Entities;
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
        { AssetClass.Create(nameof(DomainName)), DomainName.TryParse("example.com").Value },
        { AssetClass.Create(nameof(HttpEndpoint)), HttpEndpoint.TryParse("http://example.com:80/").Value },
        { AssetClass.Create(nameof(NetworkHost)), NetworkHost.TryParse("1.3.3.7").Value },
        { AssetClass.Create(nameof(NetworkRange)), NetworkRange.TryParse("1.3.3.0/24").Value },
        { AssetClass.Create(nameof(NetworkSocket)), NetworkSocket.TryParse("example.com:443").Value },
        { AssetClass.Create(nameof(DomainNameRecord)), DomainNameRecord.TryParse("example.com IN A 1.3.3.7").Value },
        { AssetClass.Create(nameof(Email)), Email.TryParse("mail@example.com").Value },
        { AssetClass.Create(nameof(HttpParameter)), HttpParameter.TryParse("http://1.3.3.7?x=y").Value },
    };

    private static IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();


    public static bool TryValidateTaskDefinitions(string fileName, out string? errorMessage)
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

        if (string.IsNullOrEmpty(file.Profile))
        {
            errorMessage = $"Null or empty profile name";
            return false;
        }

        if (!taskDefinitions.Any())
        {
            errorMessage = $"At least one task definition is required";
            return false;
        }

        if (taskDefinitions.Select(d => d.Name.Value).Distinct().Count() != taskDefinitions.Count())
        {
            errorMessage = "Duplicate Name";
            return false;
        }

        foreach (var definition in taskDefinitions)
        {
            if (string.IsNullOrEmpty(definition.Name.Value))
            {
                errorMessage = "Null or Empty Name";
                return false;
            }

            if (string.IsNullOrEmpty(definition.Subject.Value))
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

    public static bool TryValidateNotificationRules(string fileName, out string? errorMessage)
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
            errorMessage = "Duplicate Name";
            return false;
        }

        foreach (var rule in notificationRules)
        {
            if (string.IsNullOrEmpty(rule.Name.Value))
            {
                errorMessage = "Null or Empty Name";
                return false;
            }

            if (string.IsNullOrEmpty(rule.Subject.Value))
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
