using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.core.Entities.Assets;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public class DomainConfig : IEntityTypeConfiguration<core.Entities.Assets.Domain>
    {
        public void Configure(EntityTypeBuilder<core.Entities.Assets.Domain> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(Domain.Name)).IsUnique();
        }
    }
}
