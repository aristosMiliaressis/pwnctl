using pwnctl.app.Assets.DTO;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Tasks.Entities;
using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;
using System.Reflection;

namespace pwnctl.app.Assets.Aggregates;

public sealed class AssetRecord : AggregateRoot<string>
{
    public Asset Asset { get; private init; }
    public List<TaskRecord> Tasks { get; private init; } = new List<TaskRecord>();
    public Program OwningProgram { get; private init; }

    public AssetRecord(Asset asset)
    {
        Asset = asset;
    }

    public AssetRecord(List<Program> programs, Asset asset)
    {
        Asset = asset;

        OwningProgram = programs.FirstOrDefault(program => program.Scope.Any(scope => scope.Matches(Asset)));

        Asset.InScope = OwningProgram != null;
    }

    public AssetDTO ToDTO()
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

        var properties = Asset.GetType()
                            .GetProperties(BindingFlags.Public 
                                         | BindingFlags.Instance
                                         | BindingFlags.DeclaredOnly)
                            .Where(p => !p.Name.EndsWith("Id")
                                    && !p.PropertyType.IsAssignableTo(typeof(Asset)))
                            .ToList();
        properties.ForEach(p => dto.Tags.Add(p.Name, p.GetValue(Asset)) );

        return dto;
    }
}