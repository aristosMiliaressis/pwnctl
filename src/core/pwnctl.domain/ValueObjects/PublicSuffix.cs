namespace pwnctl.domain.ValueObjects;

using pwnctl.kernel.BaseClasses;

public sealed class PublicSuffix : ValueObject
{
    public string Value { get; }

    private PublicSuffix(string value)
    {
        if(Uri.CheckHostName(value) != UriHostNameType.Dns)
            throw new ArgumentException("Invalid Suffix: " + value, nameof(value));

        Value = value;
    }

    public static PublicSuffix Create(string suffix)
    {
        return new PublicSuffix(suffix);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
