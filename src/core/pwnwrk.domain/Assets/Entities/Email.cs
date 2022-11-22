using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.DTO;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Targets.Entities;
using MimeKit;

namespace pwnwrk.domain.Assets.Entities
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

        public static bool TryParse(string assetText, List<Tag> tags, out Asset[] assets)
        {
            assets = null;

            assetText = assetText.StartsWith("mailto:")
                    ? assetText.Substring(7)
                    : assetText;

            assetText = assetText.StartsWith("maito:")
                    ? assetText.Substring(6)
                    : assetText;
         
            if (!MailboxAddress.TryParse(assetText, out MailboxAddress address))
                return false;

            var domain = new Domain(address.Domain);

            assets = new Asset[] 
            {
                new Email(domain, address.Address),
                domain
            };
            
            return true;
        }

        internal override bool Matches(ScopeDefinition definition)
        {
            return Domain.Matches(definition);
        }

        public override AssetDTO ToDTO()
        {
            var dto = new AssetDTO
            {
                Asset = Address,
                Tags = new Dictionary<string, object>
                {
                    {"Hostname", Hostname}
                },
                Metadata = new Dictionary<string, string>
                {
                    {"InScope", InScope.ToString()},
                    {"FoundAt", FoundAt.ToString()},
                    {"FoundBy", FoundBy }
                }
            };

            Tags.ForEach(t => dto.Tags.Add(t.Name, t.Value));

            return dto;
        }
    }
}