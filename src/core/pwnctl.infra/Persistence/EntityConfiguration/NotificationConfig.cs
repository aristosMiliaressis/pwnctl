using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Notifications.Entities;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class NotificationRuleConfig : IEntityTypeConfiguration<NotificationRule>
    {
        public void Configure(EntityTypeBuilder<NotificationRule> builder)
        {
            builder.HasKey(r => r.Id);

            builder.OwnsOne(r => r.SubjectClass).Property(s => s.Value).IsRequired();

            builder.OwnsOne(r => r.ShortName).Property(s => s.Value).IsRequired();
        }
    }

    public sealed class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Rule)
                .WithMany()
                .HasForeignKey(e => e.RuleId);

            builder.HasOne(e => e.Record)
                .WithMany(e => e.Notifications)
                .HasForeignKey(e => e.RecordId);
        }
    }
}
