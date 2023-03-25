using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Tagging.Entities;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class TagConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Record)
                .WithMany(r => r.Tags)
                .HasForeignKey(t => t.RecordId);

            builder.HasIndex(nameof(Tag.RecordId), nameof(Tag.Name)).IsUnique();
        }
    }
}
