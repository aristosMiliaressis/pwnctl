namespace pwnctl.domain.Entities;

using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Enums;
using System.Text.RegularExpressions;

public sealed class HttpEndpoint : Asset
{
    [EqualityComponent]
    public string Url { get; init; }

    public Guid SocketAddressId { get; private init; }
    public NetworkSocket Socket { get; init; }
    public Guid? BaseEndpointId { get; private init; }
    public HttpEndpoint? BaseEndpoint { get; private set; }
    public List<HttpParameter> HttpParameters { get; private set; }
    
    public bool IsIpBased => new Regex(@"^https?://[\d]{1,3}(\.[\d]{1,3}){3}").Match(Url).Success;

    public string Scheme { get; init; }
    public string Path { get; init; }

    private HttpEndpoint() {}

    internal HttpEndpoint(string scheme, NetworkSocket address, string path)
    {
        Scheme = scheme;
        Socket = address;
        Path = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;
        Path = string.IsNullOrEmpty(Path) ? "/" : Path;

        string hostSegment = Socket.NetworkHost is not null ? Socket.NetworkHost.IP : Socket.DomainName!.Name;
        string portSegment = (scheme == "http" && Socket.Port == 80) || (scheme == "https" && Socket.Port == 443) ? "" : (":" + Socket.Port);
        Url = Scheme+"://"+hostSegment+portSegment+Path;
    }

    public static Result<HttpEndpoint, string> TryParse(string assetText)
    {
        try
        {
            if (!(assetText.ToLower().StartsWith("http") && assetText.Contains("://"))
                && !assetText.StartsWith("//"))
                return $"{assetText} is not a {nameof(HttpEndpoint)}";

            var uri = new Uri(assetText);

            // if url is protocol relative, treat it as an https url
            string scheme = uri.Port == -1 ? "https" : uri.Scheme;
            ushort port = (ushort) (uri.Port == -1 ? 443 : uri.Port);

            var result = NetworkHost.TryParse(uri.Host);
            var socket = result.Failed
                    ? new NetworkSocket(new DomainName(uri.Host), port) 
                    : new NetworkSocket(result.Value, port);

            var endpoint = new HttpEndpoint(scheme, socket, uri.AbsolutePath);

            var _params = uri.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped)
                            .Split("&")
                            .Select(p => new KeyValuePair<string, string?>(p.Split("=")[0], p.Contains("=") ? p.Split("=")[1] : null))
                            .DistinctBy(p => p.Key)
                            .Select(p => new HttpParameter(endpoint, p.Key, ParamType.Query, p.Value))
                            .Where(p => !string.IsNullOrEmpty(p.Name))
                            .ToList();

            endpoint.HttpParameters = _params;

            if (endpoint.Path != "/")
            {
                endpoint.BaseEndpoint = new HttpEndpoint(endpoint.Scheme, endpoint.Socket, "/");
            }

            return endpoint;
        }
        catch
        {
            return $"{assetText} is not a {nameof(HttpEndpoint)}";
        }
    }

    public override string ToString()
    {
        return Url;
    }
}