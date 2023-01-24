﻿using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;
using System.Net.Sockets;
using System.Net;

namespace pwnctl.domain.Entities
{
    public sealed class Host : Asset
    {
        [EqualityComponent]
        public string IP { get; init; }
        public AddressFamily Version { get; init; }

        public List<DNSRecord> AARecords { get; internal set; } = new List<DNSRecord>();
        public List<Service> Services { get; private init; }
        
        public Host() {}

        public Host(IPAddress address)
        {
            IP = address.ToString();
            Version = address.AddressFamily;
        }

        public static bool TryParse(string assetText, out Asset asset)
        {
            asset = null;

            // if inside square brakets could be an ipv6
            assetText = assetText.StartsWith("[") && assetText.EndsWith("]")
                    ? assetText.Substring(1, assetText.Length-2)
                    : assetText;

            if (assetText.Contains("]"))
                return false;

            if (IPAddress.TryParse(assetText, out IPAddress address))
            {
                var host = new Host(address);
                asset = host;
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return IP;
        }
    }
}