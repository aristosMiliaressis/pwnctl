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
        public string Hostname  => Address.Split("@").Last();
        public Domain Domain { get; private init; }
        public string DomainId { get; private init; }

        public Email() { }

        public Email(Domain domain, string address)
        {
            Address = address;
            Domain = domain;
        }

        public static bool TryParse(string assetText, out Asset asset)
        {
            asset = null;

            assetText = assetText.StartsWith("mailto:")
                    ? assetText.Substring(7)
                    : assetText;

            assetText = assetText.StartsWith("maito:")
                    ? assetText.Substring(6)
                    : assetText;

            // for some reason MailKit parses IPv4 as valid email address so if it is ip return false
            if (IPAddress.TryParse(assetText, out IPAddress _)
             || NetRange.TryParse(assetText, out Asset _))
                return false;

            if (!MailboxAddress.TryParse(assetText, out MailboxAddress address))
                return false;

            var domain = new Domain(address.Domain);

            asset = new Email(domain, address.Address);

            return true;
        }

        public override string ToString()
        {
            return Address;
        }
    }
}
