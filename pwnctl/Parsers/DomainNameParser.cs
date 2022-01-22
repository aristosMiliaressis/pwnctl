using pwnctl.Repositories;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace pwnctl.Parsers
{
    public static class DomainNameParser
    {
        private static readonly PublicSuffixRepository _suffixRepo = new PublicSuffixRepository();
        private static readonly Regex DomainRegex = new Regex("(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]");

        public static string GetRegistrationDomain(string domain)
        {
            var suffix = GetPublicSuffix(domain);
            return domain.Substring(0, domain.Length-suffix.Length-1).Split(".").Last() + "." + suffix;
        }

        public static string GetPublicSuffix(string domain)
        {
            return _suffixRepo.GetSuffixes().Where(suffix => domain.EndsWith($".{suffix}")).OrderByDescending(s => s.Length).First();
        }

        public static bool IsValid(string asset)
        {
            return DomainRegex.Match(asset).Success;
        }
    }
}