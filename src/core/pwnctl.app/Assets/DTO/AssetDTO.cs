using System.Collections;
using System.Reflection;
using pwnctl.app.Assets.Entities;
using pwnctl.domain.BaseClasses;

namespace pwnctl.app.Assets.DTO
{
    /// <summary>
    /// A DTO used to import and export assets along with their associated metadata.
    /// </summary>
    public sealed class AssetDTO
    {
        public string Asset { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public string InScope { get; set; }
        public string FoundAt { get; set; }
        public string FoundBy { get; set; }

        public AssetDTO() {}

        public AssetDTO(AssetRecord record)
        {
            {
                Asset = record.Asset.ToString();
                Tags = record.Tags.ToDictionary(t => t.Name, t => t.Value);
                InScope = record.InScope.ToString();
                FoundAt = record.FoundAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ff");
                FoundBy = record.FoundByTask?.Definition?.Name.Value ?? "N/A";

                var properties = record.Asset.GetType()
                                    .GetProperties(BindingFlags.Public
                                                 | BindingFlags.Instance
                                                 | BindingFlags.DeclaredOnly)
                                    .Where(p => !p.Name.EndsWith("Id")
                                            && !p.PropertyType.IsAssignableTo(typeof(Asset))
                                            && !p.PropertyType.IsAssignableTo(typeof(ICollection)))
                                    .ToList();

                properties.ForEach(p =>
                {
                    var val = p.GetValue(record.Asset);
                    if (val is null)
                        return;

                    var strVal = p.PropertyType.IsEnum
                        ? Enum.GetName(p.PropertyType, val)
                        : val.ToString();

                    if (strVal is null)
                        return;
                    
                    Tags.Add(p.Name, strVal);
                });
            }
        }
    }
}
