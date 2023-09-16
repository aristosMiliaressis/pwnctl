using pwnctl.app.Assets.Entities;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Tagging.Entities
{
    public sealed class Tag : Entity<int>
    {
        public string Name { get; private init; }
        public string Value { get; set; }

        public AssetRecord Record { get; set; }
        public Guid RecordId { get; set; }

        private Tag() {}

        public Tag(AssetRecord record, string name, string value)
        {
            RecordId = record.Id;
            Name = name.ToLower();
            Value = value;
        }
    }
}
