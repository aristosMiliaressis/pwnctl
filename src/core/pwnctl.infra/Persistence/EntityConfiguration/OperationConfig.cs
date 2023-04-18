using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Notifications.Entities;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class OperationConfig : IEntityTypeConfiguration<Operation>
    {
        public void Configure(EntityTypeBuilder<Operation> builder)
        {
            builder.HasKey(p => p.Id);

            builder.OwnsOne(e => e.ShortName).Property(e => e.Value).IsRequired();

            builder.HasOne(p => p.Scope)
                .WithOne()
                .HasForeignKey<Operation>(p => p.ScopeId);

            builder.HasOne(p => p.Policy)
                .WithOne()
                .HasForeignKey<Operation>(p => p.PolicyId);
        }
    }

    public sealed class PolicyConfig : IEntityTypeConfiguration<Policy>
    {
        public void Configure(EntityTypeBuilder<Policy> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.TaskProfile)
                .WithMany()
                .HasForeignKey(p => p.TaskProfileId);
        }
    }
}
