using pwnctl.Configuration;
using pwnctl.ValueObject;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace pwnctl.Repositories
{
    public class CachedPublicSuffixRepository  // TODO: use in-memory cache
    {
        private List<PublicSuffix> _publicSuffixes;
        private static CachedPublicSuffixRepository _singleton;

        public static CachedPublicSuffixRepository Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new CachedPublicSuffixRepository();
    
                return _singleton;
            }
        }
        public List<PublicSuffix> GetSuffixes()
        {
            if (_publicSuffixes == null)
            {
                    _publicSuffixes = File.ReadLines($"{EnvironmentVariables.INSTALL_PATH}/dns/public_suffix_list.dat")
                        .Select(suffix => PublicSuffix.Create(suffix))
                        .Distinct()
                        .ToList();
            }
    
            return _publicSuffixes;
        }
    }
}
