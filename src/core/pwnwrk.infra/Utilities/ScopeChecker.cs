using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Targets.Entities;

namespace pwnwrk.infra.Utilities
{
    public sealed class ScopeChecker
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

        public Program GetApplicableProgram(BaseAsset asset)
        {
            var scope = _scopeDefinitions.FirstOrDefault(d => asset.Matches(d));

            return scope?.Program;
        }
    }
}
