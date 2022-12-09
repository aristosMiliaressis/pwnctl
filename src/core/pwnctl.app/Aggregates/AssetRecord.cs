using pwnctl.app.DTO;
using pwnctl.app.Entities;
using pwnctl.app.Interfaces;
using pwnctl.domain.BaseClasses;

namespace pwnctl.app.Aggregates;

public sealed class AssetRecord
{
    public Asset Asset { get; private set; }
    public List<TaskRecord> Tasks { get; set; } = new List<TaskRecord>();

    public AssetRecord(Asset asset)
    {
        Asset = asset;
    }

    public string Serialize()
    {
        var dto = new AssetDTO();

        dto.Asset = Asset.ToString();
        dto.Tags = Asset.Tags.ToDictionary(t => t.Name, t => (object)t.Value);
        dto.Metadata = new Dictionary<string, string>
        {
            { nameof(Asset.InScope), Asset.InScope.ToString() },
            { nameof(Asset.FoundAt), Asset.FoundAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ff") },
            { nameof(Asset.FoundBy), Asset.FoundBy }
        };

        var properties = Asset.GetType().GetProperties().Where(p => !p.Name.EndsWith("Id")
                                    && !p.PropertyType.IsAssignableTo(typeof(Asset))).ToList();
        properties.ForEach(p => dto.Tags.Add(p.Name, p.GetValue(dto.Asset)));

        return Serializer.Instance.Serialize(dto);
    }
}