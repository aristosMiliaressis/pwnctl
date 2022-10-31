using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Entities;

namespace pwnwrk.infra.Utilities
{
    public class ScopeChecker
    {
        private readonly List<ScopeDefinition> _scopeDefinitions;

        public ScopeChecker(List<ScopeDefinition> scopeDefinitions)
        {
            _scopeDefinitions = scopeDefinitions;
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
