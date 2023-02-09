using pwnctl.app.Common.Interfaces;
using pwnctl.app.Assets.Aggregates;

namespace pwnctl.infra.Configuration
{
    public class MockFilterEvaluator : FilterEvaluator
    {
        public bool Evaluate(string script, AssetRecord record)
        {
            return false;
        }
    }
}