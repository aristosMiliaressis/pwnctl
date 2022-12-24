namespace pwnctl.domain.ValueObjects;

using System.Text.Json.Serialization;
using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;

public sealed class AssetClass : ValueObject
{
    public string Class { get; }

    private AssetClass(string @class)
    {
        if (!_assetClasses.Contains(@class))
            throw new ArgumentException("Invalid Asset Class: " + @class, nameof(@class));

        Class = @class;
    }

    public static AssetClass Create(string @class)
    {
        return new AssetClass(@class);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Class;
    }

    private static readonly IEnumerable<string> _assetClasses =
                    typeof(Asset).Assembly.GetTypes()
                    .Where(t => t.IsAssignableTo(typeof(Asset)))
                    .Select(t => t.Name);
}
