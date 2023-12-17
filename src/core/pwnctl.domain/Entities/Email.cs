namespace pwnctl.domain.Entities;

using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.BaseClasses;
using MimeKit;
using System.Net;

public sealed class Email : Asset
{
    [EqualityComponent]
    public string Address { get; init; }
    public DomainName DomainName { get; private init; }
    public Guid DomainId { get; private init; }

    private Email() { }

    internal Email(DomainName domain, string address)
    {
        Address = address;
        DomainName = domain;
    }

    public static Result<Email, string> TryParse(string assetText)
    {
        try
        {
            assetText = assetText.StartsWith("mailto:")
                    ? assetText.Substring(7)
                    : assetText;

            assetText = assetText.StartsWith("maito:")
                    ? assetText.Substring(6)
                    : assetText;

            // for some reason MailKit parses IPv4 as valid email address so if it is ip return false
            if (IPAddress.TryParse(assetText, out IPAddress? _) || NetworkRange.TryParse(assetText).IsOk)
                return $"{assetText} is not a {nameof(Email)}";

            if (!MailboxAddress.TryParse(assetText, out MailboxAddress address))
                return $"{assetText} is not a {nameof(Email)}";

            var domain = new DomainName(address.Domain);
            return new Email(domain, address.Address);
        }
        catch
        {
            return $"{assetText} is not a {nameof(Email)}";
        }
    }

    public override string ToString()
    {
        return Address;
    }
}