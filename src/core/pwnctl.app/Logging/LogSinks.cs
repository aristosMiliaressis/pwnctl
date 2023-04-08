namespace pwnctl.app.Logging;

[Flags]
public enum LogSinks
{
    File = 1,
    Console = 2,
    Notification = 4,
}