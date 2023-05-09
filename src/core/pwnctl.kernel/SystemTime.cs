namespace pwnctl.kernel;

public static class SystemTime
{
    public static Func<DateTime> UtcNow = () => DateTime.UtcNow;

    public static void SetDateTime(DateTime dateTimeNow)
    {
        UtcNow = () => dateTimeNow;
    }

    public static void ResetDateTime()
    {
        UtcNow = () => DateTime.UtcNow;
    }
}
