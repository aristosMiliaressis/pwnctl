using pwnctl.infra.Persistence;
using pwnctl.core.BaseClasses;
using pwnctl.core.Entities;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.app.Utilities
{
    public class ScopeChecker
    {
        private readonly PwnctlDbContext _context = new();
        private static ScopeChecker _singleton = new();
        private readonly List<ScopeDefinition> _scopeDefinitions;

        public ScopeChecker()
        {
            _scopeDefinitions = _context.ScopeDefinitions
                                .Include(d => d.Program)
                                    .ThenInclude(p => p.Policy)
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

        public Program GetApplicableProgram(BaseAsset asset)
        {
            var scope = _scopeDefinitions.FirstOrDefault(d => asset.Matches(d));

            return scope?.Program;
        }
    }
}
