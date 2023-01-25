using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class DomainNameRecordConfig : IEntityTypeConfiguration<DomainNameRecord>
    {
        public void Configure(EntityTypeBuilder<DomainNameRecord> builder)
        {
            builder.Property(e => e.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.Ignore(e => e.SPFHosts);

            builder.HasOne(e => e.DomainName)
                .WithMany(e => e.DNSRecords)
                .HasForeignKey(e => e.DomainId);

            builder.HasOne(e => e.NetworkHost)
                .WithMany(e => e.AARecords)
                .HasForeignKey(e => e.HostId);

            builder.HasIndex(nameof(DomainNameRecord.Type), nameof(DomainNameRecord.Key), nameof(DomainNameRecord.Value)).IsUnique();
        }
    }
}
