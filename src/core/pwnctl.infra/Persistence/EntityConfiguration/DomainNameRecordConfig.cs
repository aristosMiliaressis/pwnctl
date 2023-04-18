using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;
using Humanizer;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class DomainNameRecordConfig : IEntityTypeConfiguration<DomainNameRecord>
    {
        public void Configure(EntityTypeBuilder<DomainNameRecord> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.Property(e => e.Id).HasValueGenerator<UUIDv5ValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.Ignore(e => e.SPFHosts);

            builder.HasOne(e => e.DomainName)
                .WithMany()
                .HasForeignKey(e => e.DomainId);

            builder.HasOne(e => e.NetworkHost)
                .WithMany(e => e.AARecords)
                .HasForeignKey(e => e.HostId);

            builder.HasIndex(nameof(DomainNameRecord.Type), nameof(DomainNameRecord.Key), nameof(DomainNameRecord.Value)).IsUnique();
        }
    }
}
