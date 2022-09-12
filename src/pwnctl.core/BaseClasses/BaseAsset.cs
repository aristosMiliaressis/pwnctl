using pwnctl.core.Attributes;
using pwnctl.core.Entities;
using System.Reflection;

namespace pwnctl.core.BaseClasses
{
    public abstract class BaseAsset : BaseEntity
    {
        public new int Id { get; set; }
        public new DateTime FoundAt { get; set; }
        public bool InScope { get; set; }
        public bool IsRoutable { get; set; }

        public List<core.Entities.Task> Tasks { get; set; } = new List<core.Entities.Task>();
        public List<Tag> Tags { get; private set; } = new List<Tag>();
        
        public string this[string key]
        { 
            get { return Tags.FirstOrDefault(t => t.Name == key.ToLower())?.Value ?? string.Empty; }
            set {
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
    }
}
