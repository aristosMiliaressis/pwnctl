using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class DNSRecordConfig : IEntityTypeConfiguration<DNSRecord>
    {
        public void Configure(EntityTypeBuilder<DNSRecord> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(p => p.Id);

            builder.HasOne(e => e.Domain)
                .WithMany(e => e.DNSRecords)
                .HasForeignKey(e => e.DomainId);

            builder.HasOne(e => e.Host)
                .WithMany(e => e.AARecords)
                .HasForeignKey(e => e.HostId);

            builder.HasIndex(nameof(DNSRecord.Type), nameof(DNSRecord.Key)).IsUnique();
        }
    }
}
