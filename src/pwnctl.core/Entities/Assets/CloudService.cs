using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;
using pwnctl.core.Models;
using MimeKit;
using System.Text.Json;

namespace pwnctl.core.Entities.Assets
{
    public class CloudService : BaseAsset
    {
        [UniquenessAttribute]
        public string Domainname { get; set; }
        public string Service { get; set; }
        public string Provider { get; set; }
        public Domain Domain { get; set; }
        public string DomainId { get; set; }

        private CloudService() { }

        public CloudService(Domain domain)
        {
            Domainname = domain.Name;
            Domain = domain;
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            assets = null;

            return false;
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return Domain.Matches(definition);
        }

        public override string ToJson()
        {
            var dto = new AssetDTO
            {
                Asset = Domainname,
                Tags = new Dictionary<string, string>
                {
                    {"Service", Service},
                    {"Provider", Provider}
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
