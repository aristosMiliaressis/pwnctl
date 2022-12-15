namespace pwnctl.app.Tasks.Exceptions;

public sealed class TaskStateException : Exception
{
    public TaskStateException(string message)
        : base(message)
    {}
}