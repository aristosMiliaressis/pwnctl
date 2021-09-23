using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class Service : BaseAsset
    {
        public TransportProtocol TransportProtocol { get; set; }
        public string ApplicationProtocol { get; set; }
        
        public ushort Port { get; set; }
        public string IP { get; set; }
        public string Protocol { get; set; }

        public int HostId { get; set; }
        public Host Host { get; set; }

        public static bool IsNetService(string asset)
        {
            return IsIpv4ColonPort(asset) || IsIpv6ColonPort(asset);
        }

        public static bool IsIpv4ColonPort(string asset)
        {
            return asset.Split(":").Length == 2 && Host.IsIp(asset.Split(":")[0]) && ushort.TryParse(asset.Split(":")[1], out ushort port);
        }

        public static bool IsIpv6ColonPort(string asset)
        {
            var address = string.Join(":", asset.Split(":").Take(asset.Split(":").Length - 1));

            return ushort.TryParse(asset.Split(":").Last(), out ushort port) 
                && address.StartsWith("[") && address.EndsWith("]") 
                && Host.IsIpv6(address.Skip(1).Take(address.Length - 2).ToString());
        }
    }

    public enum TransportProtocol
    {
        TCP,
        UDP,
        SCTP
    }
}
