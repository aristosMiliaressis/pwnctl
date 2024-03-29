using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Operations.Entities;
using Humanizer;
using pwnctl.app.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class OperationConfig : IEntityTypeConfiguration<Operation>
    {
        public void Configure(EntityTypeBuilder<Operation> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(p => p.Id);

            builder.Property(c => c.Name)
                    .HasConversion(name => name.Value, value => ShortName.Create(value),
                    new ValueComparer<ShortName>((l, r) => l == r, v => v.GetHashCode()));

            builder.HasIndex(u => u.Name).IsUnique();

            builder.Property(c => c.Schedule)
                    .HasConversion(expr => expr.Value, value => CronExpression.Create(value),
                    new ValueComparer<CronExpression>((l, r) => l == r, v => v.GetHashCode()));

            builder.HasOne(p => p.Scope)
                .WithMany()
                .HasForeignKey(p => p.ScopeId);

            builder.HasOne(p => p.Policy)
                .WithOne()
                .HasForeignKey<Operation>(p => p.PolicyId);
        }
    }

    public sealed class PolicyConfig : IEntityTypeConfiguration<Policy>
    {
        public void Configure(EntityTypeBuilder<Policy> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(p => p.Id);

            
        }
    }

    public sealed class PolicyTaskProfileConfig : IEntityTypeConfiguration<PolicyTaskProfile>
    {
        public void Configure(EntityTypeBuilder<PolicyTaskProfile> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(p => new {p.PolicyId, p.TaskProfileId});

            builder.HasOne(p => p.Policy)
                .WithMany(p => p.TaskProfiles)
                .HasForeignKey(p => p.PolicyId);
            
            builder.HasOne(p => p.TaskProfile)
                .WithMany()
                .HasForeignKey(p => p.TaskProfileId);
        }
    }
}
