namespace pwnctl.dto.Mediator;

public sealed class MediatorError
{
    public string Message { get; set; }
    public ErrorType Type { get; set; }

    public MediatorError(ErrorType type, string message)
    {
        Type = type;
        Message = message;
    }

    public static MediatorError GenericClientError(string message) => new(ErrorType.GenericClientError, message);
    public static MediatorError InternalServerError => new(ErrorType.InternalServerError, "Internal Server Error");
    public static MediatorError Unauthorized => new(ErrorType.Unauthorized, "Client Unauthorized");

    public int ToStatusCode()
    {
        return int.Parse(((int)Type).ToString().Substring(0, 3));
    }

    public enum ErrorType
    {
        ValidationError = 400_0,
        GenericClientError = 400_9,
        Unauthorized = 401_0,
        InternalServerError = 500_0
    }
}