using Cronos;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Common.ValueObjects;

public sealed class CronExpression : ValueObject
{
    public string Value { get; }

    private CronExpression(string value)
    {
        if (!Validate(value))
            throw new ArgumentException($"Invalid CronExpression {value}, format MUST be unix 5-part", nameof(value));

        Value = value;
    }

    public DateTime? GetNextOccurrence(DateTime fromUtc)
    {
        return Cronos.CronExpression.Parse(Value).GetNextOccurrence(fromUtc);
    }

    public static CronExpression Create(string value)
    {
        return new CronExpression(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private bool Validate(string value)
    {
        if (value == null || value.Split(" ").Count() != 5)
            return false;

        try
        {
            Cronos.CronExpression.Parse(value, CronFormat.Standard);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
