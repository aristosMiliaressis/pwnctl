namespace pwnctl.app.Exceptions;

public sealed class TaskStateException : Exception
{
    public TaskStateException(string message)
        : base(message)
    {}
}