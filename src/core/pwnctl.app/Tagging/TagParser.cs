namespace pwnctl.app.Tagging;

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
            if (assetText.TrimStart().StartsWith("{"))
            {
                var dto = PwnInfraContext.Serializer.Deserialize<AssetDTO>(assetText);
                if (dto == null)
                    throw new NullReferenceException(assetText);
                return dto;
            }
            
            return new AssetDTO { Asset = assetText, FoundBy = "input" };
        }
        catch (JsonException ex)
        {
            throw new UnparsableAssetException(assetText, ex);
        }
    }
}
