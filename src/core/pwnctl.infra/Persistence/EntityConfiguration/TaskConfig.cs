using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Tasks.Entities;
using Humanizer;
using pwnctl.domain.ValueObjects;
using pwnctl.app.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using pwnctl.app;

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

            builder.Property(d => d.MonitorRules)
                    .HasConversion(
                        v => PwnInfraContext.Serializer.Serialize(v),
                        v => PwnInfraContext.Serializer.Deserialize<MonitorRules>(v));

            builder.Property(c => c.ShortName)
                    .HasConversion(name => name.Value, value => ShortName.Create(value),
                    new ValueComparer<ShortName>((l, r) => l == r, v => v.GetHashCode()));

            builder.Property(c => c.SubjectClass)
                    .HasConversion(subject => subject.Value, value => AssetClass.Create(value),
                    new ValueComparer<AssetClass>((l, r) => l == r, v => v.GetHashCode()));
        }
    }

    public sealed class TaskProfileConfig : IEntityTypeConfiguration<TaskProfile>
    {
        public void Configure(EntityTypeBuilder<TaskProfile> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(d => d.Id);

            builder.Property(c => c.ShortName)
                    .HasConversion(name => name.Value, value => ShortName.Create(value),
                    new ValueComparer<ShortName>((l, r) => l == r, v => v.GetHashCode()));

            builder.HasIndex(u => u.ShortName).IsUnique();

            builder.HasMany(p => p.TaskDefinitions)
              .WithOne(d => d.Profile)
              .HasForeignKey(d => d.ProfileId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
