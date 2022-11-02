using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Assets.DTO;
using System.Text.Json;

namespace pwnwrk.domain.Assets.Entities
{
    public sealed class CloudService : BaseAsset
    {
        [UniquenessAttribute]
        public string Hostname { get; set; }
        public string Service { get; set; }
        public string Provider { get; set; }
        public Domain Domain { get; set; }
        public string DomainId { get; set; }

        private CloudService() { }

        public CloudService(Domain domain)
        {
            Hostname = domain.Name;
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
                Asset = Hostname,
                Tags = new Dictionary<string, object>
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
