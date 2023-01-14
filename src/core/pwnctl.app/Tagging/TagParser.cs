using System.Text.Json;
using pwnctl.app;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Assets.Exceptions;

public static class TagParser
{
    public static Dictionary<string, object> Parse(ref string assetText)
    {
        Dictionary<string, object> tags = null;

        try
        {
            if (assetText.Trim().StartsWith("{"))
            {
                var dto = PwnInfraContext.Serializer.Deserialize<AssetDTO>(assetText);
                assetText = dto.Asset;
                tags = dto.Tags;
            }

            return tags;
        }
        catch (JsonException ex)
        {
            throw new UnparsableAssetException(assetText, ex);
        }
    }
}