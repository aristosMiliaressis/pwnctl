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
                .WithMany()
                .HasForeignKey(t => t.HostId);

            builder.HasOne(t => t.Service)
                .WithMany()
                .HasForeignKey(t => t.ServiceId);

            builder.HasOne(t => t.Endpoint)
                .WithMany()
                .HasForeignKey(t => t.EndpointId);

            builder.HasOne(t => t.Domain)
                .WithMany()
                .HasForeignKey(t => t.DomainId);

            builder.HasOne(t => t.NetRange)
                .WithMany()
                .HasForeignKey(t => t.NetRangeId);

            builder.HasOne(t => t.DNSRecord)
                .WithMany()
                .HasForeignKey(t => t.DNSRecordId);
        }
    }
}
