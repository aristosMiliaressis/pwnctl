namespace pwnctl.app.Tasks.Exceptions;

public sealed class CommandInterpolationException : Exception
{
    public CommandInterpolationException(string message)
        : base(message)
    { }
}