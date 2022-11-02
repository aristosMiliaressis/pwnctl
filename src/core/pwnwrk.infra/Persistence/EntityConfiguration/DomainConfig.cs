using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.infra.Persistence.IdGenerators;

namespace pwnwrk.infra.Persistence.EntityConfiguration
{
    public sealed class DomainConfig : IEntityTypeConfiguration<Domain>
    {
        public void Configure(EntityTypeBuilder<Domain> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(Domain.Name)).IsUnique();
        }
    }
}
