using pwnwrk.domain.Attributes;
using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.DTO;
using pwnwrk.domain.ValueObjects;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace pwnwrk.domain.Entities.Assets
{
    public class Domain : BaseAsset
    {
        [UniquenessAttribute]
        public string Name { get; private init; }
        public bool IsRegistrationDomain { get; private init; }
        public string RegistrationDomainId { get; private init; }
        public Domain RegistrationDomain { get; private init; }
        public List<DNSRecord> DNSRecords { get; private init; }

        private Domain() {}

        public Domain(string domain)
        {
            // support FQDNs ending with a dot.
            domain = domain.EndsWith(".") ? domain.Substring(0, domain.Length - 1) : domain;

            Name = domain;
            var regDomain = GetRegistrationDomain();
            IsRegistrationDomain = regDomain == domain;
            if (!IsRegistrationDomain)
            {
                RegistrationDomain = new Domain(regDomain);
            }
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            assets = null;

            try
            {
                if (assetText.Trim().Contains(" ") 
                 || assetText.Contains("/")
                 || assetText.Contains("*")
                 || assetText.Contains("@"))
                    return false;

                if (!_domainRegex.Match(assetText).Success)
                    return false;

                var domain = new Domain(assetText);
                domain.AddTags(tags);

                assets = new BaseAsset[] { domain };
                if (domain.RegistrationDomain != null)
                {
                    var parentDomain = domain.RegistrationDomain.Name;
                    var subs = domain.Name.Replace(parentDomain, "")
                                .Split(".")
                                .Where(sub => !string.IsNullOrEmpty(sub))
                                .Skip(1).Reverse().ToList();
                                
                    foreach (var sub in subs)
                    {
                        parentDomain = sub + "." + parentDomain;
                        assets = assets.Append(new Domain(parentDomain)).ToArray();
                    }
                    assets = assets.Append(domain.RegistrationDomain).ToArray();
                }

                var regDomain = domain.GetRegistrationDomain();
                var pubSuffix = domain.GetPublicSuffix();
                var word = regDomain.Substring(0, regDomain.Length - pubSuffix.Suffix.Length - 1);
                assets = assets.Append(new Keyword(domain.IsRegistrationDomain ? domain : domain.RegistrationDomain, word)).ToArray();

                return true;
            }
            catch
            {
                assets = null;
            }            

            return false;
        }

        public string GetRegistrationDomain()
        {
            var suffix = GetPublicSuffix();
            if (suffix == null)
                return null;

            return Name
                    .Substring(0, Name.Length - suffix.Suffix.Length - 1)
                    .Split(".")
                    .Last() + "." + suffix.Suffix;
        }

        public PublicSuffix GetPublicSuffix()
        {
            return PwnwrkDomainShim.PublicSuffixRepository.GetSuffixes()
                         .Where(suffix => Name.EndsWith($".{suffix.Suffix}"))
                         .OrderByDescending(s => s.Suffix.Length)
                         .FirstOrDefault();
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return definition.Type == ScopeDefinition.ScopeType.DomainRegex 
                && new Regex(definition.Pattern).Matches(Name).Count > 0;
        }

        public override string ToJson()
        {
            var dto = new AssetDTO
            {
                Asset = Name,
                Tags = new Dictionary<string, object>
                {
                    {"IsRegistrationDomain", IsRegistrationDomain.ToString()}
                },
                Metadata = new Dictionary<string, string>
                {
                    {"InScope", InScope.ToString() },
                    {"FoundAt", FoundAt.ToString() },
                    {"FoundBy", FoundBy }
                }
            };

            Tags.ForEach(t => dto.Tags.Add(t.Name, t.Value));

            return JsonSerializer.Serialize(dto);
        }

        private static readonly Regex _domainRegex = new Regex("(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]");
    }
}
