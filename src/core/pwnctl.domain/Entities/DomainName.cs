using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using System.Text.RegularExpressions;
using pwnctl.domain.Interfaces;

namespace pwnctl.domain.Entities
{
    public sealed class DomainName : Asset
    {
        [EqualityComponent]
        public string Name { get; init; }
        public int ZoneDepth { get; private init; }
        public DomainName ParentDomain { get; private set; }
        public Guid? ParentDomainId { get; private init; }
        public string Word => Name.Replace("."+PublicSuffixRepository.Instance.GetSuffix(Name).Value, "")
                                    .Split(".")
                                    .Last();

        public DomainName() {}

        public DomainName(string domain)
        {
            // support FQDN notation ending with a dot.
            domain = domain.EndsWith(".") ? domain.Substring(0, domain.Length - 1) : domain;

            Name = domain;

            var suffix = PublicSuffixRepository.Instance.GetSuffix(Name);
            ZoneDepth = Name.Substring(0, Name.Length - suffix.Value.Length - 1)
                        .Split(".")
                        .Count();
        }

        public static DomainName TryParse(string assetText)
        {
            try
            {
                if (assetText.Trim().Contains(" ")
                    || assetText.Contains("/")
                    || assetText.Contains("*")
                    || assetText.Contains("@"))
                    return null;

                if (!_domainRegex.Match(assetText).Success)
                    return null;

                var domain = new DomainName(assetText);

                var tmp = domain;
                var registrationDomain = new DomainName(domain.GetRegistrationDomain());
                while (tmp.Name != registrationDomain.Name)
                {
                    tmp.ParentDomain = new DomainName(string.Join(".", tmp.Name.Split(".").Skip(1)));
                    tmp = tmp.ParentDomain;
                }

                return domain;
            }
            catch
            {
                return null;
            }
       }

        public override string ToString()
        {
            return Name;
        }

        public string GetRegistrationDomain()
        {
            var suffix = PublicSuffixRepository.Instance.GetSuffix(Name);
            if (suffix == null)
                return null;

            return Name
                    .Substring(0, Name.Length - suffix.Value.Length - 1)
                    .Split(".")
                    .Last() + "." + suffix.Value;
        }

        private static readonly Regex _domainRegex = new Regex("(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]");
    }
}
