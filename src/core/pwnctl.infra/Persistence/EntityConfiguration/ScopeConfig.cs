using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Scope.Entities;
using Humanizer;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class ScopeDefinitionConfig : IEntityTypeConfiguration<ScopeDefinition>
    {
        public void Configure(EntityTypeBuilder<ScopeDefinition> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(p => p.Id);
        }
    }

    public sealed class ScopeAggregateConfig : IEntityTypeConfiguration<ScopeAggregate>
    {
        public void Configure(EntityTypeBuilder<ScopeAggregate> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(p => p.Id);

            builder.OwnsOne(e => e.ShortName).Property(e => e.Value).IsRequired();
        }
    }

    public sealed class ScopeDefinitionAggregateConfig : IEntityTypeConfiguration<ScopeDefinitionAggregate>
    {
        public void Configure(EntityTypeBuilder<ScopeDefinitionAggregate> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(p => new { p.AggregateId, p.DefinitionId });

            builder.HasOne(e => e.Aggregate)
                .WithMany(a => a.Definitions)
                .HasForeignKey(e => e.AggregateId);

            builder.HasOne(e => e.Definition)
                .WithMany()
                .HasForeignKey(e => e.DefinitionId);
        }
    }
}
