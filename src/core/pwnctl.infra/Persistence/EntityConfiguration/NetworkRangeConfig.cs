using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class NetworkRangeConfig : IEntityTypeConfiguration<NetworkRange>
    {
        public void Configure(EntityTypeBuilder<NetworkRange> builder)
        {
            builder.ToTable("network_ranges");

            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(NetworkRange.FirstAddress), nameof(NetworkRange.NetPrefixBits)).IsUnique();
        }
    }
}
