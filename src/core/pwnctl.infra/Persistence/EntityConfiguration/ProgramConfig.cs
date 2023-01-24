using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Notifications.Entities;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class ProgramConfig : IEntityTypeConfiguration<Program>
    {
        public void Configure(EntityTypeBuilder<Program> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasMany(p => p.Scope)
              .WithOne(d => d.Program)
              .HasForeignKey(d => d.ProgramId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Policy)
                .WithOne();
        }
    }

    public sealed class ScopeDefinitionConfig : IEntityTypeConfiguration<ScopeDefinition>
    {
        public void Configure(EntityTypeBuilder<ScopeDefinition> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }


    public sealed class OperationalPolicyConfig : IEntityTypeConfiguration<OperationalPolicy>
    {
        public void Configure(EntityTypeBuilder<OperationalPolicy> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }

    public sealed class NotificationRuleConfig : IEntityTypeConfiguration<NotificationRule>
    {
        public void Configure(EntityTypeBuilder<NotificationRule> builder)
        {
            builder.HasKey(r => r.Id);

            builder.OwnsOne(r => r.SubjectClass).Property(s => s.Class).IsRequired();
        }
    }
}