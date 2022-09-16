using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.core.Entities.Assets;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public class DNSRecordConfig : IEntityTypeConfiguration<DNSRecord>
    {
        public void Configure(EntityTypeBuilder<DNSRecord> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(p => p.Id);

            builder.HasOne(e => e.Domain)
                .WithMany()
                .HasForeignKey(e => e.DomainId);

            builder.HasOne(e => e.Host)
                .WithMany()
                .HasForeignKey(e => e.HostId);

            builder.HasIndex(nameof(DNSRecord.Type), nameof(DNSRecord.Key)).IsUnique();
        }
    }
}
