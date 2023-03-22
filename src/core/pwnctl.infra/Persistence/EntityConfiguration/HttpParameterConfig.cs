using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class HttpParameterConfig : IEntityTypeConfiguration<HttpParameter>
    {
        public void Configure(EntityTypeBuilder<HttpParameter> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<UUIDv5ValueGenerator>();

            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Endpoint)
                .WithMany(e => e.HttpParameters)
                .HasForeignKey(p => p.EndpointId);

            builder.HasIndex(nameof(HttpParameter.Url), nameof(HttpParameter.Name), nameof(HttpParameter.Type)).IsUnique();
        }
    }
}
