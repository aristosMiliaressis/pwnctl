namespace pwnwrk.domain.Tasks.Exceptions;

public class TaskStateException : Exception
{
    public TaskStateException(string message)
        : base(message)
    {}
}