using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Humanizer;
using pwnctl.app.Notifications.Entities;
using pwnctl.domain.ValueObjects;
using pwnctl.app.Common.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class NotificationRuleConfig : IEntityTypeConfiguration<NotificationRule>
    {
        public void Configure(EntityTypeBuilder<NotificationRule> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(r => r.Id);

            builder.Property(c => c.SubjectClass)
                    .HasConversion(subject => subject.Value, value => AssetClass.Create(value),
                    new ValueComparer<AssetClass>((l, r) => l == r, v => v.GetHashCode()));

            builder.Property(c => c.ShortName)
                    .HasConversion(name => name.Value, value => ShortName.Create(value),
                    new ValueComparer<ShortName>((l, r) => l == r, v => v.GetHashCode()));

            builder.HasIndex(u => u.ShortName).IsUnique();
        }
    }

    public sealed class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Rule)
                .WithMany()
                .HasForeignKey(e => e.RuleId);

            builder.HasOne(e => e.Task)
                .WithMany()
                .HasForeignKey(e => e.TaskId);

            builder.HasOne(e => e.Record)
                .WithMany(e => e.Notifications)
                .HasForeignKey(e => e.RecordId);
        }
    }
}
