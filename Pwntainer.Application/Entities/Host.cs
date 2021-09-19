using Pwntainer.Application.ValueObject;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class Host : BaseAsset
    {
        public string IP { get; set; }

        public OperatingSystem OperatingSystem { get; set; }

        public static bool IsIp(string asset)
        {
            return Service.IsIpv4ColonPort(asset) || IsIpv4(asset) || IsIpv6(asset);
        }

        public static bool IsIpv4(string asset)
        {
            return asset.Split(".").Length == 4 && asset.Split(".").All(p => ushort.TryParse(p, out ushort r));
        }

        public static bool IsIpv6(string asset)
        {
            // TODO is ipv6 regex
            return asset.Split(":").Count() >= 8;
        }
    }
}
