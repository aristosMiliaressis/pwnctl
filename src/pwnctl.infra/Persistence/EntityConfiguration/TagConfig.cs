using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.core.Entities;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public class TagConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Host)
                .WithMany(a => a.Tags)
                .HasForeignKey(t => t.HostId);

            builder.HasOne(t => t.Service)
                .WithMany(a => a.Tags)
                .HasForeignKey(t => t.ServiceId);

            builder.HasOne(t => t.Endpoint)
                .WithMany(a => a.Tags)
                .HasForeignKey(t => t.EndpointId);

            builder.HasOne(t => t.Domain)
                .WithMany(a => a.Tags)
                .HasForeignKey(t => t.DomainId);

            builder.HasOne(t => t.NetRange)
                .WithMany(a => a.Tags)
                .HasForeignKey(t => t.NetRangeId);

            builder.HasOne(t => t.DNSRecord)
                .WithMany(a => a.Tags)
                .HasForeignKey(t => t.DNSRecordId);
        }
    }
}
