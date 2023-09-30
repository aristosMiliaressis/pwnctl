namespace pwnctl.domain.ValueObjects;

using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;

public readonly record struct AssetClass
{
    public string Value { get; }

    private AssetClass(string value)
    {
        if (!_assetClasses.Contains(value))
            throw new ArgumentException("Invalid Asset Class: " + value, nameof(value));

        Value = value;
    }

    public static AssetClass Create(string value)
    {
        return new AssetClass(value);
    }

    private static readonly IEnumerable<string> _assetClasses =
                    typeof(Asset).Assembly.GetTypes()
                    .Where(t => t.IsAssignableTo(typeof(Asset)))
                    .Select(t => t.Name);
}
