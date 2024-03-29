﻿namespace pwnctl.domain.Entities;

using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.BaseClasses;
using System.Net;

public sealed class NetworkRange : Asset
{
    [EqualityComponent]
    public string FirstAddress { get; init; }
    [EqualityComponent]
    public ushort NetPrefixBits { get; init; }

    public string CIDR => FirstAddress+"/"+NetPrefixBits;

    private NetworkRange() {}
    
    internal NetworkRange(IPAddress firstAddress, ushort netPrefix)
    {
        FirstAddress = firstAddress.ToString();
        NetPrefixBits = netPrefix;
    }

    public static Result<NetworkRange, string> TryParse(string assetText)
    {
        try
        {
            var firstAddress = IPAddress.Parse(assetText.Split("/")[0]);
            var netPrefixBits = ushort.Parse(assetText.Split("/")[1]);
            var netRange = new NetworkRange(firstAddress, netPrefixBits);

            return netRange;
        }
        catch
        {
            return $"{assetText} is not a {nameof(NetworkRange)}";
        }
    }

    public static bool RoutesTo(string ipAddress, string cidr)
    {
        try
        {
            string[] parts = cidr.Split('/');

            int ipAddr = BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes(), 0);
            int cidrAddr = BitConverter.ToInt32(IPAddress.Parse(parts[0]).GetAddressBytes(), 0);
            int cidrMask = IPAddress.HostToNetworkOrder(-1 << (32 - int.Parse(parts[1])));

            return ((ipAddr & cidrMask) == (cidrAddr & cidrMask));
        }
        catch
        {
            return false;
        }
    }

    public override string ToString()
    {
        return CIDR;
    }
}