using pwnwrk.infra.Persistence;
using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace pwnwrk.cli.Utilities
{
    public class ScopeChecker
    {
        private static ScopeChecker _singleton = new();
        private readonly List<ScopeDefinition> _scopeDefinitions;

        public ScopeChecker()
        {
            PwnctlDbContext context = new();
            _scopeDefinitions = context.ScopeDefinitions
                                .Include(d => d.Program)
                                    .ThenInclude(p => p.Policy)
                                .AsNoTracking()
                                .ToList();
        }

        public static ScopeChecker Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new ScopeChecker();
    
                return _singleton;
            }
        }

        public bool IsInScope(BaseAsset asset)
        {
            return _scopeDefinitions.Any(d => asset.Matches(d));
        }

        public pwnwrk.domain.Entities.Program GetApplicableProgram(BaseAsset asset)
        {
            var scope = _scopeDefinitions.FirstOrDefault(d => asset.Matches(d));

            return scope?.Program;
        }
    }
}
