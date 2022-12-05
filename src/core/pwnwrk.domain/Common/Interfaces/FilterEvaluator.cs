using pwnwrk.domain.Assets.BaseClasses;

namespace pwnwrk.domain.Common.Interfaces;

public interface FilterEvaluator
{
    static FilterEvaluator Instance { get; set; }

    public bool Evaluate(string filter, Asset asset);
}