using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Tagging.Entities;
using Humanizer;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class TagConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Record)
                .WithMany(r => r.Tags)
                .HasForeignKey(t => t.RecordId);

            builder.HasIndex(nameof(Tag.RecordId), nameof(Tag.Name)).IsUnique();
        }
    }
}
