namespace pwnwrk.domain.Tasks.Exceptions;

public class CommandInterpolationException : Exception
{
    public CommandInterpolationException(string message)
        : base(message)
    { }
}