namespace pwnctl.domain.ValueObjects;

using pwnctl.kernel.BaseClasses;

public sealed class DomainName : ValueObject
{
    public string Name { get; }

    private DomainName(string name)
    {
        if (Uri.CheckHostName(name) != UriHostNameType.Dns)
            throw new ArgumentException("Invalid Domain Name: " + name, nameof(name));

        Name = name;
    }

    public static DomainName Create(string name)
    {
        return new DomainName(name);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
