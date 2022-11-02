using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.infra.Persistence.IdGenerators;

namespace pwnwrk.infra.Persistence.EntityConfiguration
{
    public sealed class EndpointConfig : IEntityTypeConfiguration<Endpoint>
    {
        public void Configure(EntityTypeBuilder<Endpoint> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId);

            builder.HasIndex(nameof(Endpoint.Url)).IsUnique();
        }
    }
}
