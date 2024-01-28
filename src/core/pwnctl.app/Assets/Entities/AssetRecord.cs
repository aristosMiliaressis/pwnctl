using pwnctl.app.Scope.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;
using pwnctl.app.Tagging.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.domain.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.kernel;

namespace pwnctl.app.Assets.Entities;

public sealed class AssetRecord : Entity<Guid>
{
    public Asset Asset => (Asset)(typeof(AssetRecord).GetProperty(Subject.Value)?.GetValue(this) ?? throw new Exception("AssetRecord reference not loaded."));
    public string TextNotation { get; set; }

    public DateTime FoundAt { get; private init; }
    public TaskRecord? FoundByTask { get; private init; }
    public int? FoundByTaskId { get; private init; }

    public bool InScope { get; private set; }
    public ScopeDefinition? Scope { get; set; }
    public int? ScopeId { get; private set; }

    public List<Tag> Tags { get; private init; } = new List<Tag>();
    public List<TaskRecord> Tasks { get; private init; } = new List<TaskRecord>();
    public List<Notification> Notifications { get; private init; } = new List<Notification>();
    public AssetClass Subject { get; set; }

    public NetworkHost? NetworkHost { get; private init; }
    public Guid? NetworkHostId { get; private init; }

    public NetworkSocket? NetworkSocket { get; private init; }
    public Guid? NetworkSocketId { get; private init; }

    public HttpEndpoint? HttpEndpoint { get; private init; }
    public Guid? HttpEndpointId { get; private init; }

    public DomainName? DomainName { get; private init; }
    public Guid? DomainNameId { get; private init; }

    public DomainNameRecord? DomainNameRecord { get; private init; }
    public Guid? DomainNameRecordId { get; private init; }

    public NetworkRange? NetworkRange { get; private init; }
    public Guid? NetworkRangeId { get; private init; }

    public Email? Email { get; private init; }
    public Guid? EmailId { get; private init; }

    public HttpParameter? HttpParameter { get; private init; }
    public Guid? HttpParameterId { get; private init; }

    public VirtualHost? VirtualHost { get; private init; }
    public Guid? VirtualHostId { get; private init; }

    // constructs immutable domain layer 
    // reference graph from assets TextNotation
    // preventing the need to JOIN tables
    private AssetRecord(string textNotation)
    {
        var result = AssetParser.Parse(textNotation);
        if (result.Failed)
            throw new Exception($"TextNotation {textNotation} is not valid");

        if (result.Value is HttpEndpoint ep && ep.HttpParameters.Any())
        {
            Subject = AssetClass.Create(nameof(HttpParameter));
            typeof(AssetRecord).GetProperty(Subject.Value)!.SetValue(this, ep.HttpParameters.First());
            return;
        }

        Subject = AssetClass.Create(result.Value.GetType().Name);
        typeof(AssetRecord).GetProperty(Subject.Value)!.SetValue(this, result.Value);
    }

    public AssetRecord(Asset asset)
    {
        TextNotation = asset.ToString();
        Subject = AssetClass.Create(asset.GetType().Name);
        typeof(AssetRecord).GetProperty(Subject.Value)!.SetValue(this, asset);
    }

    public AssetRecord(Asset asset, TaskRecord foundBy)
        : this(asset)
    {
        FoundByTaskId = foundBy?.Id;
        FoundAt = SystemTime.UtcNow();
    }

    public void SetScopeId(int scopeId)
    {
        ScopeId = scopeId;
        InScope = true;
    }

    public void MergeTags(Dictionary<string, string>? tags, bool updateExisting)
    {
        if (tags is null)
            return;

        tags.ToList().ForEach(t =>
        {
            // if a property with the tag name exists on the asset class, set that property instead of adding a tag.
            var property = typeof(AssetRecord).GetProperties().FirstOrDefault(p => p.Name.ToLower() == t.Key.ToLower());
            if (property is not null)
            {
                if (property.GetValue(this) == default)
                    property.SetValue(this, t.Value);
                return;
            }

            if (string.IsNullOrEmpty(t.Value))
                return;

            var existingTag = Tags.FirstOrDefault(eT => eT.Name == t.Key.ToLower());
            if (existingTag is not null)
            {
                if (updateExisting)
                    existingTag.Value = t.Value;
                return;
            }

            var tag = new Tag(this, t.Key, t.Value);
            Tags.Add(tag);
        });
    }

    public string this[string key]
    {
        get { return Tags.FirstOrDefault(t => t.Name == key.ToLower())?.Value ?? string.Empty; }
    }
}
