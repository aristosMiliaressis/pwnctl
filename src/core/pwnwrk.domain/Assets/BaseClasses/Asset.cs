using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.DTO;
using pwnwrk.domain.Assets.Interfaces;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Tasks.Entities;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Common.Entities;
using System.Reflection;
using System.Text.Json.Serialization;

namespace pwnwrk.domain.Assets.BaseClasses
{
    public abstract class Asset : Entity<string>
    {
        public DateTime FoundAt { get; set; }
        public string FoundBy { get; set; }
        public bool InScope { get; set; }

        public List<TaskRecord> Tasks { get; private set; } = new List<TaskRecord>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
        
        public string this[string key]
        { 
            get { return Tags.FirstOrDefault(t => t.Name == key.ToLower())?.Value ?? string.Empty; }
            set {
                // if a property with the tag name exists on the asset class, set that property instead of adding a tag.
                var property = this.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLower() == key.ToLower());
                if (property != null && property.GetValue(this) == default)
                {
                    property.SetValue(this, value);
                    return;
                }

                if (string.IsNullOrEmpty(value))
                    return;

                var tag = new Tag(key, value);
                Tags.Add(tag);
            }
        }

        public void AddTags(List<Tag> tags)
        {
            if (tags == null)
                return;

            tags.ForEach(t => this[t.Name] = t.Value);
        }

        [JsonIgnore]
        public string DomainIdentifier => string.Join(",", GetType().GetProperties().Where(p => p.GetCustomAttribute(typeof(UniquenessAttribute)) != null).Select(p => p.GetValue(this).ToString()));

        internal abstract bool Matches(ScopeDefinition definition);

        public Program GetOwningProgram(List<Program> programs)
        {
            var program = programs.FirstOrDefault(program => program.Scope.Any(scope => Matches(scope)));

            InScope = program != null;

            return program;
        }

        public List<TaskDefinition> GetMatchingTaskDefinitions(List<TaskDefinition> definitions)
        {
            return definitions
                    .Where(definition => definition.Subject == GetType().Name)
                    .Where(definition => string.IsNullOrEmpty(definition.Filter)
                                     || IFilterEvaluator.Instance.Evaluate(definition.Filter, this))
                    .ToList();
        }

        // converts asset to the AssetDTO class and serializes it to JSON
        public abstract AssetDTO ToDTO();
    }
}
