using Cronos;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Common.ValueObjects;

public sealed record CronExpression
{
    public string Value { get; }

    public CronExpression(string? value)
    {
        if (!Validate(value))
            throw new ArgumentException($"Invalid CronExpression {value}, format MUST be unix 5-part", nameof(value));

        Value = value;
    }

    public DateTime? GetNextOccurrence(DateTime fromUtc)
    {
        fromUtc = DateTime.SpecifyKind(fromUtc, DateTimeKind.Utc);
        return Cronos.CronExpression.Parse(Value).GetNextOccurrence(fromUtc);
    }

    public static CronExpression Create(string value)
    {
        return new CronExpression(value);
    }

    private bool Validate(string? value)
    {
        if (value is null || value.Split(" ").Count() != 5)
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
