using pwnctl.app.Scope.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;
using pwnctl.app.Tagging.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.domain.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.kernel;

namespace pwnctl.app.Assets.Aggregates;

public sealed class AssetRecord : Entity<Guid>
{
    public Asset Asset => (Asset)typeof(AssetRecord).GetProperty(SubjectClass.Value).GetValue(this);

    public DateTime FoundAt { get; private init; }
    public TaskEntry FoundByTask { get; private init; }
    public int? FoundByTaskId { get; private init; }

    public bool InScope { get; private set; }
    public ScopeDefinition Scope { get; set; }
    public int? ScopeId { get; private set; }

    public List<Tag> Tags { get; private init; } = new List<Tag>();
    public List<TaskEntry> Tasks { get; private init; } = new List<TaskEntry>();
    public List<Notification> Notifications { get; private init; } = new List<Notification>();
    public AssetClass SubjectClass { get; set; }

    public NetworkHost NetworkHost { get; private init; }
    public Guid? NetworkHostId { get; private init; }

    public NetworkSocket NetworkSocket { get; private init; }
    public Guid? NetworkSocketId { get; private init; }

    public HttpEndpoint HttpEndpoint { get; private init; }
    public Guid? HttpEndpointId { get; private init; }

    public DomainName DomainName { get; private init; }
    public Guid? DomainNameId { get; private init; }

    public DomainNameRecord DomainNameRecord { get; private init; }
    public Guid? DomainNameRecordId { get; private init; }

    public NetworkRange NetworkRange { get; private init; }
    public Guid? NetworkRangeId { get; private init; }

    public Email Email { get; private init; }
    public Guid? EmailId { get; private init; }

    public HttpParameter HttpParameter { get; private init; }
    public Guid? HttpParameterId { get; private init; }

    public HttpHost HttpHost { get; private init; }
    public Guid? HttpHostId { get; private init; }

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
        FoundAt = SystemTime.UtcNow();
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
