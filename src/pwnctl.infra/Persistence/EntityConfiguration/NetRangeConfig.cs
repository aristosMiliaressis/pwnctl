using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.core.Entities.Assets;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public class NetRangeConfig : IEntityTypeConfiguration<NetRange>
    {
        public void Configure(EntityTypeBuilder<NetRange> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(NetRange.FirstAddress), nameof(NetRange.NetPrefixBits)).IsUnique();
        }
    }
}
