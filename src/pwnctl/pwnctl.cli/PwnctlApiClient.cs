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

        return PwnContext.Serializer.Deserialize<TResult>((JsonElement)apiResponse.Result);
    }

    public async Task Send(IMediatedRequest request)
    {
        var apiResponse = await _send(request);

        return;
    }

    private async Task<MediatedResponse> _send(IBaseMediatedRequest request)
    {
        var concreteRequestType = request.GetType();

        var json = PwnContext.Serializer.Serialize(request, concreteRequestType);
        var route = request.GetInterpolatedRoute();

        var httpRequest = new HttpRequestMessage {
            Method = MediatedRequestTypeHelper.GetVerb(concreteRequestType),
            RequestUri = new Uri(route, UriKind.Relative),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        HttpResponseMessage response = null;
        try
        {
            response = await _httpClient.SendAsync(httpRequest,
                                            regionName: "us-east-1", // TODO: get this from profile
                                            serviceName: "lambda",
                                            credentials: GetAWSProfileCredentials(PwnContext.Config.Aws.Profile));

            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
        }

        return await response.Content.ReadFromJsonAsync<MediatedResponse>();
    }

    private static AWSCredentials GetAWSProfileCredentials(string profile)
    {
        var credentialProfileStoreChain = new CredentialProfileStoreChain();

        AWSCredentials defaultCredentials;
        if (credentialProfileStoreChain.TryGetAWSCredentials(profile, out defaultCredentials))
            return defaultCredentials;
        
        throw new AmazonClientException($"Unable to find profile {profile} in CredentialProfileStoreChain.");
    }
}