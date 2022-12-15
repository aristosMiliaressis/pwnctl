using pwnctl.kernel.Attributes;
using pwnctl.kernel.BaseClasses;
using pwnctl.domain.Entities;
using System.Reflection;
using System.Text.Json.Serialization;

namespace pwnctl.domain.BaseClasses
{
    public abstract class Asset : Entity<string>
    {
        public DateTime FoundAt { get; set; }
        public string FoundBy { get; set; }
        public bool InScope { get; set; }

        [JsonIgnore]
        public string DomainIdentifier => string.Join(",", GetType().GetProperties().Where(p => p.GetCustomAttribute(typeof(EqualityComponentAttribute)) != null).Select(p => p.GetValue(this).ToString()));

        public List<Tag> Tags { get; set; } = new List<Tag>();

        public string this[string key]
        {
            get { return Tags.FirstOrDefault(t => t.Name == key.ToLower())?.Value ?? string.Empty; }
            set
            {
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

        public abstract override string ToString();
    }
}
