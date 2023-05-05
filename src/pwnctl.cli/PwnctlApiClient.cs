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
using System.Net.Http.Headers;
using pwnctl.dto.Auth;

/// <summary>
/// an api client that utilizes the custom Mediated Api contract and
/// requires no extra implementation to support api extensions
/// </summary>
public sealed class PwnctlApiClient
{
    private readonly HttpClient _httpClient;

    public static PwnctlApiClient Default = new();

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

        HttpResponseMessage response = null;
        try
        {
            var token = await GetAccessToken();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            response = await _httpClient.SendAsync(httpRequest);

            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
        }

        return await response.Content.ReadFromJsonAsync<MediatedResponse>();
    }

    private async Task<TokenGrantResponse> GetAccessToken()
    {
        AccessTokenRequestModel request = new();

        var (profile, credentials) = TryGetAWSProfileCredentials(PwnInfraContext.Config.Aws.Profile);
        if (profile == null)
        {
            Console.WriteLine("Enter Username: ");
            request.Username = Console.ReadLine();

            Console.WriteLine("Enter Password: ");
            request.Password = Console.ReadLine();
        }
        else
        {
            request.Username = "admin";

            // TODO: get password from secrets manager
        }

        var json = PwnInfraContext.Serializer.Serialize(request);

        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/auth/token", UriKind.Relative),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(httpRequest);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TokenGrantResponse>();
    }

    private (CredentialProfile, AWSCredentials) TryGetAWSProfileCredentials(string profileName)
    {
        try
        {
            var credentialProfileStoreChain = new CredentialProfileStoreChain();

            if (!credentialProfileStoreChain.TryGetProfile(profileName, out CredentialProfile profile))
                throw new AmazonClientException($"Unable to find profile {profile} in CredentialProfileStoreChain.");

            if (!credentialProfileStoreChain.TryGetAWSCredentials(profileName, out AWSCredentials credentials))
                throw new AmazonClientException($"Unable to find credentials {profile} in CredentialProfileStoreChain.");

            return (profile, credentials);
        }
        catch
        {
            return (null, null);
        }
    }
}
