using pwnctl.Parsers;
using System;
using System.Collections.Generic;

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
            IsRegistrationDomain = DomainNameParser.GetRegistrationDomain(domain) == domain;
        }
    }
}
