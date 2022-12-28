namespace pwnctl.app.Common.Interfaces;

using pwnctl.app.Assets.Aggregates;

public interface FilterEvaluator
{
    static FilterEvaluator Instance { get; set; }

    bool Evaluate(string filter, AssetRecord record);
}