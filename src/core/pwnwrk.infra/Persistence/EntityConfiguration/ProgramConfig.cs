using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwnwrk.infra.Persistence.EntityConfiguration
{
    public class ProgramConfig : IEntityTypeConfiguration<Program>
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

    public class ScopeDefinitionConfig : IEntityTypeConfiguration<ScopeDefinition>
    {
        public void Configure(EntityTypeBuilder<ScopeDefinition> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }


    public class OperationalPolicyConfig : IEntityTypeConfiguration<OperationalPolicy>
    {
        public void Configure(EntityTypeBuilder<OperationalPolicy> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }

    public class NotificationRuleConfig : IEntityTypeConfiguration<NotificationRule>
    {
        public void Configure(EntityTypeBuilder<NotificationRule> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
