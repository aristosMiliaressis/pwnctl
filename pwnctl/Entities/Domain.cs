using pwnctl.Repositories;
using pwnctl.ValueObject;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace pwnctl.Entities
{
    public class Domain : BaseAsset, IAsset
    {
        public string Name { get; set; }
        public bool IsRegistrationDomain { get; set; }
        public int? RegistrationDomainId { get; set; }
        public Domain RegistrationDomain { get; set; }
        public List<DNSRecord> DNSRecords { get; set; }

        private Domain() {}

        public Domain(string domain)
        {
            Name = domain;
            IsRegistrationDomain = Domain.GetRegistrationDomain(domain) == domain;
        }

        public static bool TryParse(string assetText, out Domain domain)
        {
            if (_domainRegex.Match(assetText).Success)
            {
                domain = new Domain(assetText);
                return true;
            }

            domain = null;
            return false;
        }

        public static string GetRegistrationDomain(string domain)
        {
            var suffix = GetPublicSuffix(domain);

            return domain
                    .Substring(0, domain.Length-suffix.Suffix.Length-1)
                    .Split(".")
                    .Last() + "." + suffix.Suffix;
        }

        public static PublicSuffix GetPublicSuffix(string domain)
        {
            return CachedPublicSuffixRepository.Singleton.GetSuffixes()
                        .Where(suffix => domain.EndsWith($".{suffix.Suffix}"))
                        .OrderByDescending(s => s.Suffix.Length)
                        .FirstOrDefault();
        }

        private static readonly Regex _domainRegex = new Regex("(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]");
    }
}
