using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class Endpoint : BaseAsset
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public string Uri { get; set; }

        public static bool IsUrl(string asset)
        {
            return asset.Contains("://");
        }

        public static bool IsUNC(string asset)
        {
            return asset.StartsWith("//") || asset.StartsWith("\\");
        }
    }
}
