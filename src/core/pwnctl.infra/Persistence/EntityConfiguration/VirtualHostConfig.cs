using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;
using Humanizer;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class VirtualHostConfig : IEntityTypeConfiguration<VirtualHost>
    {
        public void Configure(EntityTypeBuilder<VirtualHost> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.Property(c => c.Id).HasValueGenerator<UUIDv5ValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Socket)
                .WithMany()
                .HasForeignKey(e => e.SocketId);

            builder.HasOne(e => e.Domain)
                .WithMany()
                .HasForeignKey(e => e.DomainId);

            builder.HasIndex(nameof(VirtualHost.SocketAddress), nameof(VirtualHost.Hostname)).IsUnique();
        }
    }
}
