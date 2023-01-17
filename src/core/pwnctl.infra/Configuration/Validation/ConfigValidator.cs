namespace pwnctl.infra.Configuration.Validation;

using System.Text.RegularExpressions;
using pwnctl.app;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel.Extensions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class ConfigValidator
{
    private static readonly Regex _shortNameCharSet = new Regex("^[a-zA-Z0-9_]+$");

    private static Dictionary<AssetClass, Asset> _mockAssets = new Dictionary<AssetClass, Asset>
    {
        { AssetClass.Create(nameof(Domain)), new Domain("example.com") },
        { AssetClass.Create(nameof(Endpoint)), new Endpoint("http", new Service(new Domain("example.com"), 80), "/") },
        { AssetClass.Create(nameof(Host)), new Host() },
        { AssetClass.Create(nameof(NetRange)), new NetRange() },
        { AssetClass.Create(nameof(Service)), new Service() },
        { AssetClass.Create(nameof(DNSRecord)), new DNSRecord() },
        { AssetClass.Create(nameof(Email)), new Email() },
        { AssetClass.Create(nameof(Parameter)), new Parameter() },
    };

    private static IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();


    public static bool TryValidateTaskDefinitions(string fileName, out string errorMessage)
    {
        errorMessage = null;

        List<TaskDefinition> taskDefinitions;
        try
        {
            var taskText = File.ReadAllText(fileName);
            taskDefinitions = _deserializer.Deserialize<List<TaskDefinition>>(taskText);
        }
        catch
        {
            errorMessage = $"Deserialization of {fileName} failed";
            return false;
        }
        
        if (!taskDefinitions.Any())
        {
            errorMessage = $"At least one task definition is required";
            return false;
        }

        if (taskDefinitions.Select(d => d.ShortName).Distinct().Count() != taskDefinitions.Count())
        {
            errorMessage = "Duplicate ShortName";
            return false;
        }

        foreach (var definition in taskDefinitions)
        {
            if (string.IsNullOrEmpty(definition.ShortName))
            {
                errorMessage = "Null or Empty ShortName";
                return false;
            }

            if (!_shortNameCharSet.Match(definition.ShortName).Success)
            {
                errorMessage = "Illegal Character in ShortName " + definition.ShortName;
                return false;
            }            

            if (definition.SubjectClass == null || string.IsNullOrEmpty(definition.SubjectClass.Class))
            {
                errorMessage = "Null or Empty Subject on " + definition.ShortName;
                return false;
            }

            AssetRecord record;
            try
            {
                var asset = _mockAssets[definition.SubjectClass];
                record = new AssetRecord(asset);
                var taskEntry = new TaskEntry(definition, record);
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
            if (string.IsNullOrEmpty(rule.ShortName))
            {
                errorMessage = "Null or Empty ShortName";
                return false;
            }

            if (!_shortNameCharSet.Match(rule.ShortName).Success)
            {
                errorMessage = "Illegal Character in ShortName " + rule.ShortName;
                return false;
            }

            if (rule.SubjectClass == null || string.IsNullOrEmpty(rule.SubjectClass.Class))
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