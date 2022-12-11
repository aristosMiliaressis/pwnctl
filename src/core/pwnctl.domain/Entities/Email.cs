using pwnctl.domain.Attributes;
using pwnctl.domain.BaseClasses;
using MimeKit;

namespace pwnctl.domain.Entities
{
    public sealed class Email : Asset
    {
        [UniquenessAttribute]
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

        public static bool TryParse(string assetText, out Asset mainAsset, out Asset[] relatedAssets)
        {
            mainAsset = null;
            relatedAssets = null;

            assetText = assetText.StartsWith("mailto:")
                    ? assetText.Substring(7)
                    : assetText;

            assetText = assetText.StartsWith("maito:")
                    ? assetText.Substring(6)
                    : assetText;
         
            if (!MailboxAddress.TryParse(assetText, out MailboxAddress address))
                return false;

            var domain = new Domain(address.Domain);

            mainAsset = new Email(domain, address.Address);
            relatedAssets = new Asset[] 
            {
                domain
            };
            
            return true;
        }

        public override string ToString()
        {
            return Address;
        }
    }
}
