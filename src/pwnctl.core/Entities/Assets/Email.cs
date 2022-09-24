using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;
using pwnctl.core.Models;
using MimeKit;
using System.Text.Json;

namespace pwnctl.core.Entities.Assets
{
    public class Email : BaseAsset
    {
        [UniquenessAttribute]
        public string Address { get; set; }
        public string Domainname  => Address.Split("@").Last();
        public Domain Domain { get; set; }
        public string DomainId { get; set; }

        private Email() { }

        public Email(Domain domain, string address)
        {
            Address = address;
            Domain = domain;
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            assets = null;
            if (!MailboxAddress.TryParse(assetText, out MailboxAddress address))
                return false;

            var domain = new Domain(address.Domain);

            assets = new BaseAsset[] 
            {
                new Email(domain, address.Address),
                domain
            };
            
            return true;
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return Domain.Matches(definition);
        }

        public override string ToJson()
        {
            var dto = new AssetDTO
            {
                Asset = Address,
                Tags = new Dictionary<string, string>
                {
                    {"Domain", Domainname}
                },
                Metadata = new Dictionary<string, string>
                {
                    {"InScope", InScope.ToString()},
                    {"FoundAt", FoundAt.ToString()},
                    {"FoundBy", FoundBy }
                }
            };

            Tags.ForEach(t => dto.Tags.Add(t.Name, t.Value));

            return JsonSerializer.Serialize(dto);
        }
    }
}
