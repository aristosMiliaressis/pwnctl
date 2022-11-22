﻿using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Targets.Enums;
using System.Net.Sockets;
using System.Net;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.DTO;

namespace pwnwrk.domain.Assets.Entities
{
    public sealed class Host : Asset
    {
        [UniquenessAttribute]
        public string IP { get; init; }
        public AddressFamily Version { get; init; }

        public List<DNSRecord> AARecords { get; private init; } = new List<DNSRecord>();

        public Host() {}

        public Host(string ip)
        {
            if (!IPAddress.TryParse(ip, out IPAddress address))
                throw new ArgumentException($"{ip} not a valid ip", nameof(ip));

            IP = ip;
            Version = address.AddressFamily;
        }

        public Host(string ip, AddressFamily version = AddressFamily.InterNetwork)
        {
            IP = ip;
            Version = version;
        }

        public static bool TryParse(string assetText, List<Tag> tags, out Asset[] assets)
        {
            if (IPAddress.TryParse(assetText, out IPAddress address))
            {
                var host = new Host(assetText, address.AddressFamily);
                host.AddTags(tags);
                assets = new Asset[] { host };
                return true;
            }

            assets = null;
            return false;
        }

        internal override bool Matches(ScopeDefinition definition)
        {
            return (definition.Type == ScopeType.CIDR && NetRange.RoutesTo(IP, definition.Pattern))
            || (definition.Type == ScopeType.DomainRegex && AARecords.Any(r => r.Domain.Matches(definition)));
        }

        public override AssetDTO ToDTO()
        {
            var dto = new AssetDTO
            {
                Asset = IP,
                Tags = new Dictionary<string, object>
                {
                    {"Version", Version.ToString()}
                },
                Metadata = new Dictionary<string, string>
                {
                    {"InScope", InScope.ToString()},
                    {"FoundAt", FoundAt.ToString()},
                    {"FoundBy", FoundBy }
                }
            };

            Tags.ForEach(t => dto.Tags.Add(t.Name, t.Value));

            return dto;
        }
    }
}