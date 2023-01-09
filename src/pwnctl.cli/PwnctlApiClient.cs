namespace pwnctl.cli;

using System;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using pwnctl.dto.Mediator;
using pwnctl.app;
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
        _httpClient.BaseAddress = new Uri(PwnInfraContext.Config.Api.BaseUrl);
    }

    public async Task<TResult> Send<TResult>(MediatedRequest<TResult> request)
    {
        var apiResponse = await _send(request);
        if (apiResponse.Result == null)
            return default;

        return PwnInfraContext.Serializer.Deserialize<TResult>((JsonElement)apiResponse.Result);
    }

    public async Task Send(MediatedRequest request)
    {
        var apiResponse = await _send(request);

        return;
    }

    private async Task<MediatedResponse> _send(Request request)
    {
        var concreteRequestType = request.GetType();

        var json = PwnInfraContext.Serializer.Serialize(request, concreteRequestType);
        var route = request.GetInterpolatedRoute();

        var httpRequest = new HttpRequestMessage {
            Method = MediatedRequestTypeHelper.GetVerb(concreteRequestType),
            RequestUri = new Uri(route, UriKind.Relative),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        
        var (profile, credentials) = GetAWSProfileCredentials(PwnInfraContext.Config.Aws.Profile);

        HttpResponseMessage response = null;
        try
        {
            response = await _httpClient.SendAsync(httpRequest,
                                            regionName: profile.Region.SystemName,
                                            serviceName: "lambda",
                                            credentials: credentials);
            
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
        }

        return await response.Content.ReadFromJsonAsync<MediatedResponse>();
    }

    private static (CredentialProfile, AWSCredentials) GetAWSProfileCredentials(string profileName)
    {
        var credentialProfileStoreChain = new CredentialProfileStoreChain();

        if (!credentialProfileStoreChain.TryGetProfile(profileName, out CredentialProfile profile))
            throw new AmazonClientException($"Unable to find profile {profile} in CredentialProfileStoreChain.");

        if (!credentialProfileStoreChain.TryGetAWSCredentials(profileName, out AWSCredentials credentials))
            throw new AmazonClientException($"Unable to find credentials {profile} in CredentialProfileStoreChain.");

        return (profile, credentials);
    }
}