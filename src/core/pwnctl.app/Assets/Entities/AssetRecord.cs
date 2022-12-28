using pwnctl.app.Assets.DTO;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;
using System.Collections;
using System.Reflection;
using pwnctl.app.Tagging.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.domain.Entities;

namespace pwnctl.app.Assets.Aggregates;

public sealed class AssetRecord : Entity<string>
{
    public Asset Asset =>(Asset)typeof(AssetRecord).GetProperty(SubjectClass.Class).GetValue(this);
    
    public DateTime FoundAt { get; set; }
    public string FoundBy { get; set; }
    public bool InScope { get; set; }

    public List<Tag> Tags { get; private init; } = new List<Tag>();
    public List<TaskEntry> Tasks { get; private init; } = new List<TaskEntry>();
    public Program OwningProgram { get; private set; }
    public AssetClass SubjectClass { get; private set; }

    public Host Host { get; set; }
    public string HostId { get; set; }

    public Service Service { get; set; }
    public string ServiceId { get; set; }

    public Endpoint Endpoint { get; set; }
    public string EndpointId { get; set; }

    public Domain Domain { get; set; }
    public string DomainId { get; set; }

    public DNSRecord DNSRecord { get; set; }
    public string DNSRecordId { get; set; }

    public NetRange NetRange { get; set; }
    public string NetRangeId { get; set; }

    public Keyword Keyword { get; set; }
    public string KeywordId { get; set; }

    public Email Email { get; set; }
    public string EmailId { get; set; }

    public CloudService CloudService { get; set; }
    public string CloudServiceId { get; set; }

    public Parameter Parameter { get; set; }
    public string ParameterId { get; set; }

    public VirtualHost VirtualHost { get; set; }
    public string VirtualHostId { get; set; }

    private AssetRecord() {}

    public AssetRecord(Asset asset)
    {
        SubjectClass = AssetClass.Create(asset.GetType().Name);
        typeof(AssetRecord).GetProperty(SubjectClass.Class).SetValue(this, asset);
    }

    public void SetOwningProgram(Program program)
    {
        OwningProgram = program;
        InScope = OwningProgram != null;
    }

    public void UpdateTags(Dictionary<string, object> tags)
    {
        if (tags == null)
            return;

        tags.ToList().ForEach(t => this[t.Key] = t.Value.ToString());
    }

    public string this[string key]
    {
        get { return Tags.FirstOrDefault(t => t.Name == key.ToLower())?.Value ?? string.Empty; }
        set
        {
            // if a property with the tag name exists on the asset class, set that property instead of adding a tag.
            var property = Asset.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLower() == key.ToLower());
            if (property != null && property.GetValue(Asset) == default)
            {
                property.SetValue(Asset, value);
                return;
            }

            if (string.IsNullOrEmpty(value))
                return;

            if (Tags.Any(t => t.Name == key.ToLower()))
                return;

            var tag = new Tag(this, key, value);
            Tags.Add(tag);
        }
    }

    public AssetDTO ToDTO()
    {
        var dto = new AssetDTO();

        dto.Asset = Asset.ToString();
        dto.Tags = Tags.ToDictionary(t => t.Name, t => (object)t.Value);
        dto.Metadata = new Dictionary<string, string>
        {
            { nameof(InScope), InScope.ToString() },
            { nameof(FoundAt), FoundAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.ff") },
            { nameof(FoundBy), FoundBy }
        };

        var properties = Asset.GetType()
                            .GetProperties(BindingFlags.Public 
                                         | BindingFlags.Instance
                                         | BindingFlags.DeclaredOnly)
                            .Where(p => !p.Name.EndsWith("Id")
                                    && !p.PropertyType.IsAssignableTo(typeof(Asset))
                                    && !p.PropertyType.IsAssignableTo(typeof(ICollection)))
                            .ToList();

        properties.ForEach(p => 
        {
            var val = p.GetValue(Asset);

            val = p.PropertyType.IsEnum
                ? Enum.GetName(p.PropertyType, val)
                : val;

            dto.Tags.Add(p.Name, val);
        });

        return dto;
    }
}