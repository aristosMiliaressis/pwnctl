using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;
using Humanizer;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class NetworkSocketConfig : IEntityTypeConfiguration<NetworkSocket>
    {
        public void Configure(EntityTypeBuilder<NetworkSocket> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.Property(c => c.Id).HasValueGenerator<UUIDv5ValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.NetworkHost)
                .WithMany()
                .HasForeignKey(e => e.NetworkHostId);

            builder.HasOne(e => e.DomainName)
                .WithMany()
                .HasForeignKey(e => e.DomainNameId);

            builder.HasIndex(nameof(NetworkSocket.Address)).IsUnique();
        }
    }
}
