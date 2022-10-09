using pwnwrk.domain.Attributes;
using pwnwrk.domain.Entities;
using System.Reflection;

namespace pwnwrk.domain.BaseClasses
{
    public abstract class BaseAsset : BaseEntity<string>
    {
        public DateTime FoundAt { get; set; }
        public string FoundBy { get; set; }
        public bool InScope { get; set; }

        public List<domain.Entities.Task> Tasks { get; set; } = new List<domain.Entities.Task>();
        public List<Tag> Tags { get; private set; } = new List<Tag>();
        
        public string this[string key]
        { 
            get { return Tags.FirstOrDefault(t => t.Name == key.ToLower())?.Value ?? string.Empty; }
            set {
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

        public abstract string ToJson();
    }
}
