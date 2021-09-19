using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class Domain : BaseAsset
    {
        private static string DnsCharset => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public string Name { get; set; }
        public bool InScope { get; set; } = true;

        public static bool IsDomain(string asset)
        {
            var parts = asset.Split(".");

            return IsDomainPort(asset) || parts.Length > 1 && parts.All(p => IsCharset(p, DnsCharset)) && IsTld(parts.Last());
        }

        public static bool IsDomainPort(string asset)
        {
            return asset.Contains(":") && IsDomain(asset.Split(":")[0]) && ushort.TryParse(asset.Split(":")[1], out ushort port);
        }

        public static bool IsWildcardDomain(string asset)
        {
            var parts = asset.Split(".");
            return parts.Length > 1 && parts.All(p => IsCharset(p, DnsCharset + "*")) && IsTld(parts.Last());
        }

        public static bool IsCharset(string asset, string charset)
        {
            var allowedCharacters = charset.ToArray().Distinct();

            return asset.All(character => allowedCharacters.Contains(character));
        }

        public static bool IsTld(string asset)
        {
            return true; // TODO: IsTld read tlds from file
            throw new NotImplementedException();
        }
    }
}
