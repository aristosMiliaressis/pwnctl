namespace pwnctl.api.Extensions;

using pwnctl.dto.Mediator;
using System.Net;
using System.Text.Json;

public static class HttpResponseExtensions
{
    public static async Task Create(this HttpResponse response, HttpStatusCode status)
    {
        var json = JsonSerializer.Serialize(MediatedResponse.Create(status));
        response.StatusCode = (int)status;
        await response.WriteAsync(json);
    }
}