namespace pwnctl.cli;

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using pwnctl.dto.Mediator;
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

    public async Task<TResult> Send<TResult>(IMediatedRequest<TResult> request)
    {
        var apiResponse = await _send(request);

        return (TResult)apiResponse.Result;
    }

    public async Task Send(IMediatedRequest request)
    {
        var apiResponse = await _send(request);

        return;
    }

    public async Task<MediatedResponse<TResult>> TrySend<TResult>(IMediatedRequest<TResult> request)
    {
        try
        {
            return (MediatedResponse<TResult>)await _send(request);
        }
        catch
        {
            return (MediatedResponse<TResult>)MediatedResponse.Create(System.Net.HttpStatusCode.InternalServerError);
        }
    }

    public async Task<MediatedResponse> TrySend(IMediatedRequest request)
    {
        try
        {
            return await _send(request);
        }
        catch
        {
            return MediatedResponse.Create(System.Net.HttpStatusCode.InternalServerError);
        }
    }

    private async Task<MediatedResponse> _send(IBaseMediatedRequest request)
    {
        var content = JsonContent.Create(request, request.GetType());
        
        var route = request.GetInterpolatedRoute();

        var response = await _httpClient.PostAsync(route, content);
        
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MediatedResponse>();;
    }
}