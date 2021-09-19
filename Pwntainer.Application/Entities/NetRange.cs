using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class NetRange : BaseAsset
    {
        public string CIDR { get; set; }

        public static bool IsCidr(string asset)
        {
            return asset.Contains("/") && Host.IsIp(asset.Split("/")[0]) && byte.TryParse(asset.Split("/").Last(), out byte c);
        }
    }
}
