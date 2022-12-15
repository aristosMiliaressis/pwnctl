namespace pwnctl.api.Extensions;

using pwnctl.dto.Mediator;
using pwnctl.app.Common.Interfaces;
using System.Net;

public static class HttpResponseExtensions
{
    public static async Task Create(this HttpResponse response, MediatedResponse result)
    {
        response.StatusCode = result.IsSuccess
                            ? (int)HttpStatusCode.OK
                            : result.Errors.MaxBy(err => err.Type).ToStatusCode();
                            
        var json = Serializer.Instance.Serialize(result);
        
        await response.WriteAsync(json);
    }

    public static async Task Create(this HttpResponse response, HttpStatusCode status)
    {
        response.StatusCode = (int)status;

        var result = MediatedResponse.Create(status);

        var json = Serializer.Instance.Serialize(result);

        await response.WriteAsync(json);
    }
}