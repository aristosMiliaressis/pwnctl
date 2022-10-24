namespace pwnctl.api.Models;

using pwnwrk.infra.MediatR;
using System.Net;

public class ApiResponse
{
    public object Result { get; init; }
    public ApiError[] Errors { get; init; }

    public ApiResponse(MediatorResponse response)
    {
        Result = response.Result;
        Errors = response.ErrorMessages.Select(msg => new ApiError(ApiError.ErrorCode.GenericClientError, msg)).ToArray();
    }

    public ApiResponse(params ApiError[] errors)
    {
        Errors = errors;
    }

    public static ApiResponse Create(HttpStatusCode status)
    {
        return status switch
        {
            HttpStatusCode.InternalServerError => InternalServerError,
            HttpStatusCode.Unauthorized => Unauthorized,
            _ => throw new NotImplementedException()
        };
    }

    private static ApiResponse InternalServerError => new ApiResponse(ApiError.InternalServerError);
    private static ApiResponse Unauthorized => new ApiResponse(ApiError.Unauthorized);
}