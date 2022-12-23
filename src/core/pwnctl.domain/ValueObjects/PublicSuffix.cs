namespace pwnctl.domain.ValueObjects;

using pwnctl.kernel.BaseClasses;

public sealed class PublicSuffix : ValueObject
{
    public string Suffix { get; }

    private PublicSuffix(string suffix)
    {
        if(Uri.CheckHostName(suffix) != UriHostNameType.Dns)
            throw new ArgumentException("Invalid Public Suffix: " + suffix, nameof(suffix));

        Suffix = suffix;
    }

    public static PublicSuffix Create(string suffix)
    {
        return new PublicSuffix(suffix);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Suffix;
    }
}
