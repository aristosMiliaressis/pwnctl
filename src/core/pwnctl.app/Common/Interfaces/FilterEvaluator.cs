namespace pwnctl.app.Common.Interfaces;

using pwnctl.app.Assets.Aggregates;

public interface FilterEvaluator
{
    bool Evaluate(string filter, AssetRecord record);
}