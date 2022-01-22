using pwnctl.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace pwnctl.Repositories
{
    public class PublicSuffixRepository
    {
        private static readonly string _suffixesFilePath = $"{EnvironmentVariables.INSTALL_PATH}/dns/public_suffix_list.dat";
        private static readonly List<string> _publicSuffixes = File.ReadLines(_suffixesFilePath).ToList();

        public List<string> GetSuffixes()
        {
            return _publicSuffixes;
        }
    }
}