namespace pwnctl.api.Models;

public class ApiError
{
    public string Message { get; set; }
    public ErrorCode Code { get; set; }

    public ApiError(ErrorCode code, string message)
    {
        Code = code;
        Message = message;
    }

    public static ApiError GenericClientError(string message) => new ApiError(ErrorCode.GenericClientError, message);
    public static ApiError InternalServerError => new ApiError(ErrorCode.InternalServerError, "Internal Server Error");
    public static ApiError Unauthorized => new ApiError(ErrorCode.Unauthorized, "Client Unauthorized");

    public int ToStatusCode()
    {
        return int.Parse(Code.ToString().Substring(0, 3));
    }

    public enum ErrorCode
    {
        GenericClientError = 400_0,
        Unauthorized = 401_0,
        InternalServerError = 500_0
    }
}