namespace pwnctl.api.Extensions;

using pwnctl.dto.Mediator;
using System.Net;
using System.Text.Json;

public static class HttpResponseExtensions
{
    public static async Task Create(this HttpResponse response, MediatedResponse result)
    {
        response.StatusCode = result.IsSuccess
                            ? (int)HttpStatusCode.OK
                            : result.Errors.MaxBy(err => err.Type).ToStatusCode();
                            
        var json = JsonSerializer.Serialize(result);
        
        await response.WriteAsync(json);
    }

    public static async Task Create(this HttpResponse response, HttpStatusCode status)
    {
        var json = JsonSerializer.Serialize(MediatedResponse.Create(status));
        response.StatusCode = (int)status;
        await response.WriteAsync(json);
    }
}