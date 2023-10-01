namespace pwnctl.app.Tagging;

using System.Text.Json;
using pwnctl.kernel.BaseClasses;
using pwnctl.kernel.Extensions;
using pwnctl.app;
using pwnctl.app.Assets.DTO;

public static class TagParser
{
    public static Result<AssetDTO, string> Parse(string assetText)
    {
        try
        {
            if (assetText.TrimStart().StartsWith("{"))
            {
                var dto = PwnInfraContext.Serializer.Deserialize<AssetDTO>(assetText);
                if (dto == null)
                    return $"Can't parse asset {assetText}";
                
                return dto;
            }
            
            return new AssetDTO { Asset = assetText, FoundBy = "input" };
        }
        catch (JsonException ex)
        {
            return $"Can't parse asset {assetText}\r\n{ex.ToRecursiveExInfo()}" ;
        }
    }
}
