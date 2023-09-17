namespace pwnctl.app.Common.Interfaces;

using pwnctl.app.Assets.Entities;

public interface FilterEvaluator
{
    bool Evaluate(string? filter, AssetRecord record);

    bool Evaluate(string? filter, Dictionary<string, object> args);
}
