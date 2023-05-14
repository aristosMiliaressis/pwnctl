using System.Text.RegularExpressions;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Common.ValueObjects;

public sealed class ShortName : ValueObject
{
    public string Value { get; }

    private ShortName(string value)
    {
        if (!_shortNameCharSet.Match(value).Success)
            throw new ArgumentException("Invalid ShortName: " + value, nameof(value));

        Value = value;
    }

    public static ShortName Create(string value)
    {
        return new ShortName(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private static readonly Regex _shortNameCharSet = new Regex("^[a-zA-Z0-9_]{0,64}$");
}
