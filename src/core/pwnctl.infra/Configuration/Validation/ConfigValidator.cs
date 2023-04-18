namespace pwnctl.infra.Configuration.Validation;

using pwnctl.app;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class ConfigValidator
{
    private static Dictionary<AssetClass, Asset> _mockAssets = new Dictionary<AssetClass, Asset>
    {
        { AssetClass.Create(nameof(DomainName)), new DomainName("example.com") },
        { AssetClass.Create(nameof(HttpEndpoint)), new HttpEndpoint("http", new NetworkSocket(new DomainName("example.com"), 80), "/") },
        { AssetClass.Create(nameof(NetworkHost)), new NetworkHost() },
        { AssetClass.Create(nameof(NetworkRange)), new NetworkRange() },
        { AssetClass.Create(nameof(NetworkSocket)), new NetworkSocket() },
        { AssetClass.Create(nameof(DomainNameRecord)), new DomainNameRecord() },
        { AssetClass.Create(nameof(Email)), new Email() },
        { AssetClass.Create(nameof(HttpParameter)), new HttpParameter() },
    };

    private static IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();


    public static bool TryValidateTaskDefinitions(string fileName, out string errorMessage)
    {
        errorMessage = null;

        TaskDefinitionFile file;
        try
        {
            var taskText = File.ReadAllText(fileName);
            file = _deserializer.Deserialize<TaskDefinitionFile>(taskText);
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

        if (!file.TaskDefinitions.Any())
        {
            errorMessage = $"At least one task definition is required";
            return false;
        }

        if (file.TaskDefinitions.Select(d => d.ShortName).Distinct().Count() != file.TaskDefinitions.Count())
        {
            errorMessage = "Duplicate ShortName";
            return false;
        }

        foreach (var definition in file.TaskDefinitions)
        {
            if (string.IsNullOrEmpty(definition.ShortName.Value))
            {
                errorMessage = "Null or Empty ShortName";
                return false;
            }

            if (definition.SubjectClass == null || string.IsNullOrEmpty(definition.SubjectClass.Value))
            {
                errorMessage = "Null or Empty Subject on " + definition.ShortName;
                return false;
            }

            AssetRecord record;
            try
            {
                var asset = _mockAssets[definition.SubjectClass];
                record = new AssetRecord(asset);
                var op = new Operation();
                var taskEntry = new TaskEntry(op, definition, record);
                var test = taskEntry.Command;
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Error(ex.ToRecursiveExInfo());
                errorMessage = $"task definition {definition.ShortName} failed to interpolate CommandTemplate arguments";
                return false;
            }

            try
            {
                definition.Matches(record);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Error(ex.ToRecursiveExInfo());
                errorMessage = $"task definition {definition.ShortName} Filter through exception";
                return false;
            }
        }

        return true;
    }

    public static bool TryValidateNotificationRules(string fileName, out string errorMessage)
    {
        errorMessage = null;

        List<NotificationRule> notificationRules;
        try
        {
            var ruleText = File.ReadAllText(fileName);
            notificationRules = _deserializer.Deserialize<List<NotificationRule>>(ruleText);
        }
        catch
        {
            errorMessage = $"Deserialization of {fileName} failed";
            return false;
        }

        if (notificationRules.Select(d => d.ShortName).Distinct().Count() != notificationRules.Count())
        {
            errorMessage = "Duplicate ShortName";
            return false;
        }

        foreach (var rule in notificationRules)
        {
            if (string.IsNullOrEmpty(rule.ShortName.Value))
            {
                errorMessage = "Null or Empty ShortName";
                return false;
            }

            if (rule.SubjectClass == null || string.IsNullOrEmpty(rule.SubjectClass.Value))
            {
                errorMessage = "Null or Empty Subject on " + rule.ShortName;
                return false;
            }

            AssetRecord record = new AssetRecord(_mockAssets[rule.SubjectClass]);

            try
            {
                rule.Check(record);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Error(ex.ToRecursiveExInfo());
                errorMessage = $"notification rule {rule.ShortName} Filter through exception";
                return false;
            }
        }

        return true;
    }
}