using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using MimeKit;
using System.Net;

namespace pwnctl.domain.Entities
{
    public sealed class Email : Asset
    {
        [EqualityComponent]
        public string Address { get; init; }
        public DomainName DomainName { get; private init; }
        public Guid DomainId { get; private init; }

        public Email() { }

        public Email(DomainName domain, string address)
        {
            Address = address;
            DomainName = domain;
        }

        public static Email? TryParse(string assetText)
        {
            assetText = assetText.StartsWith("mailto:")
                    ? assetText.Substring(7)
                    : assetText;

            assetText = assetText.StartsWith("maito:")
                    ? assetText.Substring(6)
                    : assetText;

            // for some reason MailKit parses IPv4 as valid email address so if it is ip return false
            if (IPAddress.TryParse(assetText, out IPAddress _)
             || NetworkRange.TryParse(assetText) is not null)
                return null;

            if (!MailboxAddress.TryParse(assetText, out MailboxAddress address))
                return null;

            var domain = new DomainName(address.Domain);
            return new Email(domain, address.Address);
        }

        public override string ToString()
        {
            return Address;
        }
    }
}
