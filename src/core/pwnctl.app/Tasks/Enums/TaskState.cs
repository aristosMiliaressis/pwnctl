namespace pwnctl.app.Tasks.Enums;

public enum TaskState
{
    QUEUED,
    RUNNING,
    FINISHED,
    FAILED,
    CANCELED,
    TIMED_OUT,
}