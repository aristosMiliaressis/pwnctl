using pwnctl.app.Common.Exceptions;

namespace pwnctl.app.Tasks.Exceptions;

public sealed class StringInterpolationException : AppException
{
    public StringInterpolationException(string message)
        : base(message)
    { }
}