using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.BaseClasses;

namespace pwnwrk.domain.Assets.Interfaces;

public interface IFilterEvaluator : IAmbientService
{
    bool Evaluate(string filter, Asset asset);
}