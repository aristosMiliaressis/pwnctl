namespace pwnctl.app.Common.Exceptions;

public class AppException : Exception
{
    public AppException(string message)
        : base(message)
    { }
}