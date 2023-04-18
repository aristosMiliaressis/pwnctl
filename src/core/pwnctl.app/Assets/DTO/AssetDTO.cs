using System.Collections;
using System.Reflection;
using pwnctl.app.Assets.Aggregates;
using pwnctl.domain.BaseClasses;

namespace pwnctl.app.Assets.DTO
{
    /// <summary>
    /// A DTO used to import and export assets along with their associated metadata.
    /// </summary>
    public sealed class AssetDTO
    {
        public string Asset { get; set; }
        public Dictionary<string, object> Tags { get; set; }
        public string InScope { get; set; }
        public string FoundAt { get; set; }
        public string FoundBy { get; set; }

        public AssetDTO() {}

        public AssetDTO(AssetRecord record)
        {
            {
                Asset = record.Asset.ToString();
                Tags = record.Tags.ToDictionary(t => t.Name, t => (object)t.Value);
                InScope = record.InScope.ToString();
                FoundAt = record.FoundAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ff");
                FoundBy = record?.FoundByTask?.Definition?.ShortName?.Value ?? "N/A";

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

                    val = p.PropertyType.IsEnum
                        ? Enum.GetName(p.PropertyType, val)
                        : val;

                    Tags.Add(p.Name, val);
                });
            }
        }
    }
}