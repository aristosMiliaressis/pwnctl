using System.Text.RegularExpressions;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.domain.ValueObjects;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public static class ConfigValidator
{
    private static readonly Regex _shortNameCharSet = new Regex("^[a-zA-Z08_-]+$");

    private static Dictionary<AssetClass, Asset> _mockAssets = new Dictionary<AssetClass, Asset>
    {
        { AssetClass.Create(nameof(Domain)), new Domain() },
        { AssetClass.Create(nameof(Endpoint)), new Endpoint() },
        { AssetClass.Create(nameof(Host)), new Host() },
        { AssetClass.Create(nameof(NetRange)), new NetRange() },
        { AssetClass.Create(nameof(Service)), new Service() },
        { AssetClass.Create(nameof(DNSRecord)), new DNSRecord() },
        { AssetClass.Create(nameof(Keyword)), new Keyword() },
        { AssetClass.Create(nameof(Email)), new Email() },
        { AssetClass.Create(nameof(Parameter)), new Parameter() },
        { AssetClass.Create(nameof(CloudService)), new CloudService() },
    };

    private static IDeserializer _deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();


    public static bool TryValidateTaskDefinitions(string fileName, out string errorMessage)
    {
        errorMessage = null;

        // 1. Deserialize
        var taskText = File.ReadAllText(fileName);
        var taskDefinitions = _deserializer.Deserialize<List<TaskDefinition>>(taskText);

        // 2. ShortNames are unique
        if (taskDefinitions.Select(d => d.ShortName).Distinct().Count() == taskDefinitions.Count())
        {
            errorMessage = "Duplicate ShortName";
            return false;
        }

        foreach (var definition in taskDefinitions)
        {
            // 3. Shortname is not null or empty
            if (string.IsNullOrEmpty(definition.ShortName))
            {
                errorMessage = "Null or Empty ShortName";
                return false;
            }

            // 4. ShortName charset
            if (!_shortNameCharSet.Match(definition.ShortName).Success)
            {
                errorMessage = "Illegal Character in ShortName " + definition.ShortName;
                return false;
            }            

            // 5. Subject is valid
            if (definition.SubjectClass == null || string.IsNullOrEmpty(definition.SubjectClass.Class))
            {
                errorMessage = "Null or Empty Subject on " + definition.ShortName;
                return false;
            }

            var record = new AssetRecord(_mockAssets[definition.SubjectClass]);
            var taskEntry = new TaskEntry(definition, record);

            // 6. Interpolation test
            var test = taskEntry.Command;

            // 7. Filter test
            definition.Matches(record);
        }

        return true;
    }
}