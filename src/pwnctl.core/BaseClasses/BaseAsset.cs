using pwnctl.core.Entities;

namespace pwnctl.core.BaseClasses
{
    public abstract class BaseAsset : BaseEntity
    {
        public new int Id { get; set; }
        public new DateTime FoundAt { get; set; }
        public bool InScope { get; set; }
        public bool IsRoutable { get; set; }

        public List<core.Entities.Task> Tasks { get; set; } = new List<core.Entities.Task>();
        public List<Tag> Tags { get; set; } = new List<Tag>();
        public string this[string key]
        { 
            get { return Tags.FirstOrDefault(t => t.Name == key)?.Value; }
        }

        public abstract bool Matches(ScopeDefinition definition);
    }
}
