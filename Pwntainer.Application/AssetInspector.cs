using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pwntainer.Core
{
    public static class AssetInspector
    {
        private static string DnsCharset => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public static bool IsUrl(string asset)
        {
            return asset.Contains("://");
        }

        public static bool IsIp(string asset)
        {
            return IsIpv4ColonPort(asset) || IsIpv4(asset) || IsIpv6(asset);
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

        public static bool IsIpColonPort(string asset)
        {
            return IsIpv4ColonPort(asset) || IsIpv6ColonPort(asset);
        }

        public static bool IsIpv4ColonPort(string asset)
        {
            return asset.Split(":").Length == 2 && IsIp(asset.Split(":")[0]) && ushort.TryParse(asset.Split(":")[1], out ushort port);
        }

        public static bool IsIpv6ColonPort(string asset)
        {
            var address = string.Join(":", asset.Split(":").Take(asset.Split(":").Length - 1));

            return ushort.TryParse(asset.Split(":").Last(), out ushort port) && address.StartsWith("[") && address.EndsWith("]") && IsIpv6(address.Skip(1).Take(address.Length - 2).ToString());
        }

        public static bool IsCidr(string asset)
        {
            return asset.Contains("/") && IsIp(asset.Split("/")[0]) && byte.TryParse(asset.Split("/").Last(), out byte c);
        }

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

        public static bool IsUNC(string asset)
        {
            throw new NotImplementedException();
        }
    }
}
