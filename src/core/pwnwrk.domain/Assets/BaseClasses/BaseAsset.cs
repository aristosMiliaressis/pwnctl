using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Tasks.Entities;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Common.Entities;
using System.Reflection;

namespace pwnwrk.domain.Assets.BaseClasses
{
    public abstract class BaseAsset : BaseEntity<string>
    {
        public DateTime FoundAt { get; set; }
        public string FoundBy { get; set; }
        public bool InScope { get; set; }

        public List<TaskRecord> Tasks { get; set; } = new List<TaskRecord>();
        public List<Tag> Tags { get; private set; } = new List<Tag>();
        
        public string this[string key]
        { 
            get { return Tags.FirstOrDefault(t => t.Name == key.ToLower())?.Value ?? string.Empty; }
            set {
                // if a property with the tag name exists on the asset class, set that property instead of adding a tag.
                var property = this.GetType().GetProperties().FirstOrDefault(p => p.Name.ToLower() == key.ToLower());
                if (property != null)
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

        public string DomainIdentifier => string.Join(",", GetType().GetProperties().Where(p => p.GetCustomAttribute(typeof(UniquenessAttribute)) != null).Select(p => p.GetValue(this).ToString()));

        public abstract bool Matches(ScopeDefinition definition);

        // converts asset to the AssetDTO sealed class and serializes it to JSON
        public abstract string ToJson();
    }
}
