namespace pwnctl.cli;

using System;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using pwnctl.dto.Mediator;
using pwnwrk.infra;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

/// <summary>
/// an api client that utilizes the custom Mediated Api contract and
/// requires no extra implementation to support api extensions
/// </summary>
public sealed class PwnctlApiClient
{
    private readonly HttpClient _httpClient;

    public PwnctlApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(PwnContext.Config.Api.BaseUrl);
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
        var concreteRequestType = request.GetType();

        var json = JsonSerializer.Serialize(request, concreteRequestType);
        var route = request.GetInterpolatedRoute();

        Console.WriteLine(route);
        var httpRequest = new HttpRequestMessage {
            Method = MediatedRequestTypeHelper.GetMethod(concreteRequestType),
            RequestUri = new Uri(route, UriKind.Relative),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(httpRequest, 
                                            regionName: "us-east-1", // TODO: get this from profile
                                            serviceName: "lambda",
                                            credentials: GetAWSCredentialsFromProfile());
        
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MediatedResponse>();;
    }

    private static AWSCredentials GetAWSCredentialsFromProfile()
    {
        var credentialProfileStoreChain = new CredentialProfileStoreChain();

        AWSCredentials defaultCredentials;
        if (credentialProfileStoreChain.TryGetAWSCredentials(PwnContext.Config.Aws.Profile, out defaultCredentials))
            return defaultCredentials;
        
        throw new AmazonClientException("Unable to find a default profile in CredentialProfileStoreChain.");
    }
}