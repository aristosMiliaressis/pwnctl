using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class HttpEndpointConfig : IEntityTypeConfiguration<HttpEndpoint>
    {
        public void Configure(EntityTypeBuilder<HttpEndpoint> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Socket)
                .WithMany(s => s.Endpoints)
                .HasForeignKey(e => e.SocketAddressId);

            builder.HasIndex(nameof(HttpEndpoint.Url)).IsUnique();

            builder.HasOne(e => e.ParentEndpoint)
                .WithMany()
                .HasForeignKey(e => e.ParentEndpointId);
        }
    }
}
