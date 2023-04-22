using pwnctl.app.Scope.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;
using pwnctl.app.Tagging.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.domain.Entities;
using pwnctl.app.Notifications.Entities;

namespace pwnctl.app.Assets.Aggregates;

public sealed class AssetRecord : Entity<Guid>
{
    public Asset Asset => (Asset)typeof(AssetRecord).GetProperty(SubjectClass.Value).GetValue(this);
    
    public DateTime FoundAt { get; set; }
    public TaskEntry FoundByTask { get; set; }
    public int? FoundByTaskId { get; set; }

    public bool InScope { get; set; }
    public ScopeDefinition Scope { get; set; }
    public int? ScopeId { get; set; }

    public List<Tag> Tags { get; private init; } = new List<Tag>();
    public List<TaskEntry> Tasks { get; private init; } = new List<TaskEntry>();
    public List<Notification> Notifications { get; private init; } = new List<Notification>();
    public AssetClass SubjectClass { get; set; }

    public NetworkHost NetworkHost { get; set; }
    public Guid? NetworkHostId { get; set; }

    public NetworkSocket NetworkSocket { get; set; }
    public Guid? NetworkSocketId { get; set; }

    public HttpEndpoint HttpEndpoint { get; set; }
    public Guid? HttpEndpointId { get; set; }

    public DomainName DomainName { get; set; }
    public Guid? DomainNameId { get; set; }

    public DomainNameRecord DomainNameRecord { get; set; }
    public Guid? DomainNameRecordId { get; set; }

    public NetworkRange NetworkRange { get; set; }
    public Guid? NetworkRangeId { get; set; }

    public Email Email { get; set; }
    public Guid? EmailId { get; set; }

    public HttpParameter HttpParameter { get; set; }
    public Guid? HttpParameterId { get; set; }

    public HttpHost HttpHost { get; set; }
    public Guid? HttpHostId { get; set; }

    private AssetRecord() {}

    public AssetRecord(Asset asset)
    {
        SubjectClass = AssetClass.Create(asset.GetType().Name);
        typeof(AssetRecord).GetProperty(SubjectClass.Value).SetValue(this, asset);
    }

    public AssetRecord(Asset asset, TaskEntry foundBy)
        : this(asset)
    {
        FoundByTaskId = foundBy?.Id;
    }

    public void SetScope(ScopeDefinition scope)
    {
        Scope = scope;
        ScopeId = scope?.Id;
        InScope = Scope != null;
    }

    public void UpdateTags(Dictionary<string, object> tags)
    {
        if (tags == null)
            return;

        tags.ToList().ForEach(t => this[t.Key] = t.Value?.ToString());
    }

    public string this[string key]
    {
        get { return Tags.FirstOrDefault(t => t.Name == key.ToLower())?.Value ?? string.Empty; }
        set
        {
            // if a property with the tag name exists on the asset class, set that property instead of adding a tag.
            var property = typeof(AssetRecord).GetProperties().FirstOrDefault(p => p.Name.ToLower() == key.ToLower());
            if (property != null)
            {
                if (property.GetValue(this) == default)
                    property.SetValue(this, value);
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
}