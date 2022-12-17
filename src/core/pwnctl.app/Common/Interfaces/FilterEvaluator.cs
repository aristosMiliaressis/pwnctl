
using pwnctl.domain.BaseClasses;

namespace pwnctl.app.Common.Interfaces;

public interface FilterEvaluator
{
    static FilterEvaluator Instance { get; set; }

    bool Evaluate(string filter, Asset asset);
}