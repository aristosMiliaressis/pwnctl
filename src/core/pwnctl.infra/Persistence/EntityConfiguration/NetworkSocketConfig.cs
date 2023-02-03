using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class NetworkSocketConfig : IEntityTypeConfiguration<NetworkSocket>
    {
        public void Configure(EntityTypeBuilder<NetworkSocket> builder)
        {
            builder.ToTable("network_sockets");

            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.NetworkHost)
                .WithMany(h => h.Sockets)
                .HasForeignKey(e => e.NetworkHostId);

            builder.HasOne(e => e.DomainName)
                .WithMany(h => h.Sockets)
                .HasForeignKey(e => e.DomainNameId);

            builder.HasIndex(nameof(NetworkSocket.Address)).IsUnique();
        }
    }
}
