using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Interfaces;
using pwnctl.domain.ValueObjects;
using System.Text.RegularExpressions;

namespace pwnctl.domain.Entities
{
    public sealed class Domain : Asset
    {
        [EqualityComponent]
        public string Name { get; init; }
        public bool IsRegistrationDomain { get; init; }
        public string RegistrationDomainId { get; private init; }
        public Domain RegistrationDomain { get; private init; }
        public List<DNSRecord> DNSRecords { get; private init; }

        public Domain() {}

        public Domain(string domain)
        {
            // support FQDN notation ending with a dot.
            domain = domain.EndsWith(".") ? domain.Substring(0, domain.Length - 1) : domain;

            Name = domain;
            var regDomain = GetRegistrationDomain();
            IsRegistrationDomain = regDomain == domain;
            if (!IsRegistrationDomain)
            {
                RegistrationDomain = new Domain(regDomain);
            }
        }

        public static bool TryParse(string assetText, out Asset mainAsset, out Asset[] relatedAssets)
        {
            relatedAssets = new Asset[] {};
            mainAsset = null;

            if (assetText.Trim().Contains(" ")
                || assetText.Contains("/")
                || assetText.Contains("*")
                || assetText.Contains("@"))
                return false;

            if (!_domainRegex.Match(assetText).Success)
                return false;

            var domain = new Domain(assetText);

            mainAsset = domain;
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
                    relatedAssets = relatedAssets.Append(new Domain(parentDomain)).ToArray();
                }
                relatedAssets = relatedAssets.Append(domain.RegistrationDomain).ToArray();
            }

            var regDomain = domain.GetRegistrationDomain();
            var pubSuffix = domain.GetPublicSuffix();
            var word = regDomain.Substring(0, regDomain.Length - pubSuffix.Suffix.Length - 1);
            relatedAssets = relatedAssets.Append(new Keyword(domain.IsRegistrationDomain ? domain : domain.RegistrationDomain, word)).ToArray();

            return true;
       }

        public override string ToString()
        {
            return Name;
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
            return PublicSuffixRepository.Instance.List()
                         .Where(suffix => Name.EndsWith($".{suffix.Suffix}"))
                         .OrderByDescending(s => s.Suffix.Length)
                         .FirstOrDefault();
        }

        private static readonly Regex _domainRegex = new Regex("(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]");
    }
}
