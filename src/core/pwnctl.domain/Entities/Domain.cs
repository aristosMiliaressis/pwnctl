﻿using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using System.Text.RegularExpressions;
using pwnctl.domain.Interfaces;

namespace pwnctl.domain.Entities
{
    public sealed class Domain : Asset
    {
        [EqualityComponent]
        public string Name { get; init; }
        public int ZoneDepth { get; private init; }
        public Domain ParentDomain { get; private set; }
        public string ParentDomainId { get; private init; }
        public List<DNSRecord> DNSRecords { get; private init; }
        public List<Service> Services { get; private init; }
        public string Word => Name.Replace($".{PublicSuffixListService.Instance.GetSuffix(Name).Value}", "")
                                    .Split(".")
                                    .Last();

        public Domain() {}

        public Domain(string domain)
        {
            // support FQDN notation ending with a dot.
            domain = domain.EndsWith(".") ? domain.Substring(0, domain.Length - 1) : domain;

            Name = domain;

            var suffix = PublicSuffixListService.Instance.GetSuffix(Name);
            ZoneDepth = Name.Substring(0, Name.Length - suffix.Value.Length - 1)
                        .Split(".")
                        .Count();
        }

        public static bool TryParse(string assetText, out Asset asset)
        {
            asset = null;

            if (assetText.Trim().Contains(" ")
                || assetText.Contains("/")
                || assetText.Contains("*")
                || assetText.Contains("@"))
                return false;

            if (!_domainRegex.Match(assetText).Success)
                return false;

            var domain = new Domain(assetText);

            var tmp = domain;
            var registrationDomain = new Domain(domain.GetRegistrationDomain());
            while (tmp.Name != registrationDomain.Name)
            {
                tmp.ParentDomain = new Domain(string.Join(".", tmp.Name.Split(".").Skip(1)));
                tmp = tmp.ParentDomain;
            }

            asset = domain;

            return true;
       }

        public override string ToString()
        {
            return Name;
        }

        public string GetRegistrationDomain()
        {
            var suffix = PublicSuffixListService.Instance.GetSuffix(Name);
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