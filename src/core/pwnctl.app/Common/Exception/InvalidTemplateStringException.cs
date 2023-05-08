using pwnctl.app.Common.Exceptions;

namespace pwnctl.app.Tasks.Exceptions;

public sealed class InvalidTemplateStringException : AppException
{
    public InvalidTemplateStringException(string message)
        : base(message)
    { }
}
