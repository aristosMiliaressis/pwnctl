namespace pwnctl.kernel.Extensions;

public static class DateTimeExtensions
{
    public static int ToEpochTime(this DateTime time)
    {
        return (int)(time - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
