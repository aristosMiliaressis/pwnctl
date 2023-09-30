namespace pwnctl.domain.ValueObjects;

using pwnctl.kernel.BaseClasses;

public readonly record struct PublicSuffix
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

    public override string ToString() 
    {
        return Value;
    }
}
