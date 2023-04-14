namespace pwnctl.app.Logging;

[Flags]
public enum LogSinks
{
    File = 1,
    Console = 2,
    CloudWatch = 4,
    Notification = 8,

    All = 15,
}