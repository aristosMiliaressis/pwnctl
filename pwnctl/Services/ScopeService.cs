using pwnctl;
using pwnctl.Persistence;
using pwnctl.Entities;
using pwnctl.Handlers;
using pwnctl.Parsers;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace pwnctl.Services
{
    public class ScopeService
    {
        private readonly PwntainerDbContext _context = new();
        private static ScopeService _singleton;

        public static ScopeService Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new ScopeService();
    
                return _singleton;
            }
        }

        public bool IsInScope(IAsset asset)
        {
            return _context.ScopeDefinitions.Any(d => d.Matches(asset));
        }
    }
}
