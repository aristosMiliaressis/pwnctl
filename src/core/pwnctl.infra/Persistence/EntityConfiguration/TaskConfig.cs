using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Tasks.Entities;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class TaskEntryConfig : IEntityTypeConfiguration<TaskEntry>
    {
        public void Configure(EntityTypeBuilder<TaskEntry> builder)
        {
            builder.ToTable("task_entries");

            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Definition)
                .WithMany()
                .HasForeignKey(t => t.DefinitionId);

            builder.HasOne(t => t.Record)
                .WithMany()
                .HasForeignKey(t => t.RecordId);
        }
    }

    public sealed class TaskDefinitionConfig : IEntityTypeConfiguration<TaskDefinition>
    {
        public void Configure(EntityTypeBuilder<TaskDefinition> builder)
        {
            builder.ToTable("task_definitions");

            builder.HasKey(d => d.Id);

            builder.OwnsOne(d => d.SubjectClass).Property(s => s.Class).IsRequired();
        }
    }
}