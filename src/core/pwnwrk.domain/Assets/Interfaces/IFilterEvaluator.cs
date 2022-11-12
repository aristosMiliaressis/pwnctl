using pwnwrk.domain.Assets.BaseClasses;

namespace pwnwrk.domain.Assets.Interfaces;

public interface IFilterEvaluator
{
    static IFilterEvaluator Instance { get; set; }

    bool Evaluate(string filter, Asset asset);
}