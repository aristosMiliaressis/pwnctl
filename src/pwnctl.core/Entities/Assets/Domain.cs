﻿using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;
using pwnctl.core.Models;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace pwnctl.core.Entities.Assets
{
    public class Domain : BaseAsset
    {
        [UniquenessAttribute]
        public string Name { get; set; }
        public bool IsRegistrationDomain { get; set; }
        public string RegistrationDomainId { get; set; }
        public Domain RegistrationDomain { get; set; }
        public List<DNSRecord> DNSRecords { get; set; }

        private Domain() {}

        public Domain(string domain)
        {
            Name = domain;
            IsRegistrationDomain = PwnctlCoreShim.PublicSuffixRepository.GetRegistrationDomain(domain) == domain;
            if (!IsRegistrationDomain)
            {
                var registrationDomain = new Domain(PwnctlCoreShim.PublicSuffixRepository.GetRegistrationDomain(domain));
                RegistrationDomain = registrationDomain;
            }
        }

        public static bool TryParse(string assetText, List<Tag> tags, out BaseAsset[] assets)
        {
            assets = null;

            try
            {
                if (assetText.Trim().Contains(" ") 
                 || assetText.Trim().Contains("/")
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
                                .Skip(1)
                                .Reverse()
                                .ToList();
                    foreach (var sub in subs)
                    {
                        parentDomain = sub + "." + parentDomain;
                        assets = assets.Append(new Domain(parentDomain)).ToArray();
                    }
                    assets = assets.Append(domain.RegistrationDomain).ToArray();
                }

                var regDomain = PwnctlCoreShim.PublicSuffixRepository.GetRegistrationDomain(domain.Name);
                var pubSuffix = PwnctlCoreShim.PublicSuffixRepository.GetPublicSuffix(domain.Name);
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
                Tags = new Dictionary<string, string>
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
