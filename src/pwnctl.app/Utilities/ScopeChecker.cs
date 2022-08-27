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
            return _context.ScopeDefinitions.ToList().Any(d => asset.Matches(d));
        }

        public Program GetApplicableProgram(BaseAsset asset)
        {
            var scope = _context.ScopeDefinitions
                                .Include(d => d.Program)
                                    .ThenInclude(p => p.Policy)
                                .ToList()
                                .FirstOrDefault(d => asset.Matches(d));

            return scope?.Program;
        }
    }
}
