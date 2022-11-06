namespace pwnctl.api.Extensions;

using pwnctl.dto.Mediator;
using pwnwrk.infra;
using System.Net;

public static class HttpResponseExtensions
{
    public static async Task Create(this HttpResponse response, MediatedResponse result)
    {
        response.StatusCode = result.IsSuccess
                            ? (int)HttpStatusCode.OK
                            : result.Errors.MaxBy(err => err.Type).ToStatusCode();
                            
        var json = PwnContext.Serializer.Serialize(result);
        
        await response.WriteAsync(json);
    }

    public static async Task Create(this HttpResponse response, HttpStatusCode status)
    {
        var json = PwnContext.Serializer.Serialize(MediatedResponse.Create(status));
        response.StatusCode = (int)status;
        await response.WriteAsync(json);
    }
}