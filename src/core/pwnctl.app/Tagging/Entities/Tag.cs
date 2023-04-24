using pwnctl.app.Assets.Aggregates;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Tagging.Entities
{
    public sealed class Tag : Entity<int>
    {
        public string Name { get; private init; }
        public string Value { get; private init; }

        public AssetRecord Record { get; private init; }
        public Guid RecordId { get; private init; }

        private Tag() {}

        public Tag(AssetRecord record, string name, string value)
        {
            Record = record;
            Name = name.ToLower();
            Value = value;
        }
    }
}