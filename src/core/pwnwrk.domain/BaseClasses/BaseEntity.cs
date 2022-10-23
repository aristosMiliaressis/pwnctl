using System.Text.Json.Serialization;

namespace pwnwrk.domain.BaseClasses
{
    public abstract class BaseEntity
    {

    }

    public abstract class BaseEntity<TPKey> : BaseEntity
    {
        protected BaseEntity()
        {
        }

        [JsonIgnore]
        public TPKey Id { get; set; }
    }
}
