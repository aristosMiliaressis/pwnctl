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
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Linq;
using System.Collections.Generic;

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
        _httpClient.Timeout = TimeSpan.FromMinutes(5);
    }

    public async Task<TResult> Send<TResult>(MediatedRequest<TResult> request)
         where TResult : class
    {
        (Type concreteRequestType, string route, string json) = await PrepareRequest(request);

        if (concreteRequestType.IsAssignableTo(typeof(PaginatedRequest)))
        {
            return await HandlePaginatedRequest<TResult>(concreteRequestType, route, json);
        }

        var apiResponse = await _send(concreteRequestType, route, json);
        if (apiResponse.Result == null)
            return default;

        return PwnInfraContext.Serializer.Deserialize<TResult>((JsonElement)apiResponse.Result);
    }

    public async Task Send(MediatedRequest request)
    {
        (Type concreteRequestType, string route, string json) = await PrepareRequest(request);
        
        var apiResponse = await _send(concreteRequestType, route, json);

        return;
    }

    private async Task<(Type concreteRequestType, string route, string json)> PrepareRequest(Request request)
    {
        var token = await GetAccessToken();

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        var concreteRequestType = request.GetType();

        var json = PwnInfraContext.Serializer.Serialize(request, concreteRequestType);
        var route = request.GetInterpolatedRoute();

        return (concreteRequestType, route, json);
    }

    private async Task<MediatedResponse> _send(Type concreteRequestType, string route, string json)
    {
        HttpResponseMessage response = null;
        try
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = MediatedRequestTypeHelper.GetVerb(concreteRequestType),
                RequestUri = new Uri(route, UriKind.Relative),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

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

    private async Task<TResult> HandlePaginatedRequest<TResult>(Type concreteRequestType, string route, string json)
     where TResult : class
    {
        MediatedResponse response = null;
        PaginatedViewModel view = null;

        do
        {
            var httpRequest = new HttpRequestMessage
            {
                Method = MediatedRequestTypeHelper.GetVerb(concreteRequestType),
                RequestUri = new Uri($"{route}?Page={(view == null ? 0 : view.Page)}", UriKind.Relative),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var httpResponse = await _httpClient.SendAsync(httpRequest);

            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();

            var responseType = typeof(MediatedResponse<TResult>);

            response = (MediatedResponse) PwnInfraContext.Serializer.Deserialize(jsonResponse, responseType);

            var viewModel = responseType.GetProperties().First(p => p.Name == "Result").GetValue(response) as PaginatedViewModel;

            if (view == null)
                view = viewModel;
            else
                view.Rows.AddRange((List<object>) typeof(PaginatedViewModel).GetProperty("Rows").GetValue(viewModel));
    
            view.Page++;
        }
        while (view.Page != view.TotalPages+1);

        return view as TResult;
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
            var ssmClient = new AmazonSecretsManagerClient();
            var password = await ssmClient.GetSecretValueAsync(new GetSecretValueRequest
            {
                SecretId = "/aws/secret/pwnctl/admin_password"
            });

            request.Username = "admin";
            request.Password = password.SecretString;
        }

        var json = PwnInfraContext.Serializer.Serialize(request);

        var httpRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/auth/grant", UriKind.Relative),
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
