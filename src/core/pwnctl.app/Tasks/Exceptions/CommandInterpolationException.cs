using pwnctl.app.Common.Exceptions;

namespace pwnctl.app.Tasks.Exceptions;

public sealed class CommandInterpolationException : AppException
{
    public CommandInterpolationException(string message)
        : base(message)
    { }
}