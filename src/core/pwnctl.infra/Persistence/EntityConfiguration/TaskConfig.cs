using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Tasks.Entities;
using Humanizer;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class TaskEntryConfig : IEntityTypeConfiguration<TaskEntry>
    {
        public void Configure(EntityTypeBuilder<TaskEntry> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Definition)
                .WithMany()
                .HasForeignKey(t => t.DefinitionId);

            builder.HasOne(t => t.Record)
                .WithMany(r => r.Tasks)
                .HasForeignKey(t => t.RecordId);

            builder.HasOne(t => t.Operation)
                .WithMany()
                .HasForeignKey(t => t.OperationId);
        }
    }

    public sealed class TaskDefinitionConfig : IEntityTypeConfiguration<TaskDefinition>
    {
        public void Configure(EntityTypeBuilder<TaskDefinition> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(d => d.Id);

            builder.OwnsOne(e => e.ShortName).Property(e => e.Value).IsRequired();

            builder.OwnsOne(d => d.SubjectClass).Property(s => s.Value).IsRequired();
        }
    }

    public sealed class TaskProfileConfig : IEntityTypeConfiguration<TaskProfile>
    {
        public void Configure(EntityTypeBuilder<TaskProfile> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(d => d.Id);

            builder.OwnsOne(e => e.ShortName).Property(e => e.Value).IsRequired();

            builder.HasMany(p => p.TaskDefinitions)
              .WithOne(d => d.Profile)
              .HasForeignKey(d => d.ProfileId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}