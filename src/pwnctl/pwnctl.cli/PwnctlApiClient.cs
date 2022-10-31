namespace pwnctl.cli;

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using pwnctl.dto.Api;
using pwnctl.dto;
using pwnwrk.infra;

public class PwnctlApiClient
{
    private readonly HttpClient _httpClient;

    public PwnctlApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(PwnContext.Config.Api.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("X-Api-Key", PwnContext.Config.Api.ApiKey);
    }

    public async Task<TResult> Send<TResult>(IApiRequest<TResult> request, params string[] routeArgs)
    {
        var apiResponse = await _send(request, routeArgs);

        return (TResult) apiResponse.Result;
    }

    public async Task<ApiResponse<TResult>> TrySend<TResult>(IApiRequest<TResult> request, params string[] routeArgs)
    {
        try
        {
            return (ApiResponse<TResult>) await _send(request, routeArgs);
        }
        catch
        {
            //  TODO: handle this properly
            return (ApiResponse<TResult>) ApiResponse.Create(System.Net.HttpStatusCode.InternalServerError);
        }        
    }

    private async Task<ApiResponse> _send<TResult>(IApiRequest<TResult> request, params string[] routeArgs)
    {
        var content = JsonContent.Create(request);

        var route = _interpolateRouteArgs(request.ReflectedConcreteRoute, routeArgs);

        var response = await _httpClient.PostAsync(route, content);
        
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ApiResponse>();;
    }

    private string _interpolateRouteArgs(string routeTemplate, params string[] routeArgs)
    {
        if (routeTemplate.Count(s => s == '{') != routeArgs.Length)
            throw new Exception("Route argument count mismatch");

        int idx = 0;
        routeTemplate.Split("{")
            .Skip(1)
            .ToList()
            .ForEach(arg =>
            {
                arg = arg.Split("}")[0];
                routeTemplate = routeTemplate.Replace("{" + arg + "}", routeArgs[idx]);
                idx++;
            });

        return routeTemplate;
    }
}