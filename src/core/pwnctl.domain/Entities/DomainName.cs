﻿namespace pwnctl.domain.Entities;

using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.BaseClasses;
using Nager.PublicSuffix;

public sealed class DomainName : Asset
{
    [EqualityComponent]
    public string Name { get; init; }
    public int ZoneDepth { get; private init; }
    public DomainName? ParentDomain { get; set; }
    public Guid? ParentDomainId { get; private init; }
    public string Word => Name.Replace("." + _domainParser.Parse(Name).TLD, "")
                                .Split(".")
                                .Last();

    private DomainName() {}

    internal DomainName(string domain)
    {
        // support FQDN notation ending with a dot.
        domain = domain.EndsWith(".") ? domain.Substring(0, domain.Length - 1) : domain;

        var parsed = _domainParser.Parse(domain);

        Name = domain;
        ZoneDepth = Name.Substring(0, Name.Length - parsed.TLD.Length - 1)
            .Split(".")
            .Count();
    }

    public static Result<DomainName, string> TryParse(string assetText)
    {
        try
        {
            if (assetText.Trim().Contains(" ")
                || assetText.Contains("/")
                || assetText.Contains("@"))
                return $"{assetText} is not a {nameof(DomainName)}, charset";

            // support FQDN notation ending with a dot.
            assetText = assetText.EndsWith(".") ? assetText.Substring(0, assetText.Length - 1) : assetText;

            if (!_domainParser.IsValidDomain(assetText))
                return $"{assetText} is not a {nameof(DomainName)}, validation";

            var domain = new DomainName(assetText);
            var regDomain = domain.GetRegistrationDomain();
            if (regDomain is null)
                return $"{assetText} is not a {nameof(DomainName)}, registration domain";

            var tmp = domain;
            var registrationDomain = new DomainName(regDomain);
            while (tmp.Name != registrationDomain.Name)
            {
                tmp.ParentDomain = new DomainName(string.Join(".", tmp.Name.Split(".").Skip(1)));
                tmp = tmp.ParentDomain;
            }

            return domain;
        }
        catch (Exception ex)
        {
            return $"{assetText} is not a {nameof(DomainName)}, {ex.Message}";
        }
    }

    public override string ToString()
    {
        return Name;
    }

    public string? GetRegistrationDomain()
    {
        var tld = _domainParser.Parse(Name).TLD;
        if (tld is null)
            return null;
        
        return Name
                .Substring(0, Name.Length - tld.Length - 1)
                .Split(".")
                .Last() + "." + tld;
    }

    private static DomainParser _domainParser = new DomainParser(new WebTldRuleProvider());
}