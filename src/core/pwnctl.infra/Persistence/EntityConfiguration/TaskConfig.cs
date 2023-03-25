using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class TaskEntryConfig : IEntityTypeConfiguration<TaskEntry>
    {
        public void Configure(EntityTypeBuilder<TaskEntry> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Definition)
                .WithMany()
                .HasForeignKey(t => t.DefinitionId);

            builder.HasOne(t => t.Record)
                .WithMany(r => r.Tasks)
                .HasForeignKey(t => t.RecordId);
        }
    }

    public sealed class TaskDefinitionConfig : IEntityTypeConfiguration<TaskDefinition>
    {
        public void Configure(EntityTypeBuilder<TaskDefinition> builder)
        {
            builder.HasKey(d => d.Id);

            builder.OwnsOne(d => d.SubjectClass).Property(s => s.Class).IsRequired();
        }
    }

    public sealed class TaskProfileConfig : IEntityTypeConfiguration<TaskProfile>
    {
        public void Configure(EntityTypeBuilder<TaskProfile> builder)
        {
            builder.HasKey(d => d.Id);

            builder.HasMany(p => p.TaskDefinitions)
              .WithOne(d => d.Profile)
              .HasForeignKey(d => d.ProfileId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}