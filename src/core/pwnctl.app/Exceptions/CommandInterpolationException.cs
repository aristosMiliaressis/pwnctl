namespace pwnctl.app.Exceptions;

public sealed class CommandInterpolationException : Exception
{
    public CommandInterpolationException(string message)
        : base(message)
    { }
}