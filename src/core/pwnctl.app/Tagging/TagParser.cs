using System.Text.Json;
using pwnctl.app;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Assets.Exceptions;

public static class TagParser
{
    public static AssetDTO Parse(string assetText)
    {
        try
        {
            return (assetText.Trim().StartsWith("{"))
                ? PwnInfraContext.Serializer.Deserialize<AssetDTO>(assetText)
                : new AssetDTO { Asset = assetText, FoundBy = "input" };
        }
        catch (JsonException ex)
        {
            throw new UnparsableAssetException(assetText, ex);
        }
    }
}