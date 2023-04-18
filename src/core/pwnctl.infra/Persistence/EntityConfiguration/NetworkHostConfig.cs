using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;
using Humanizer;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class NetworkHostConfig : IEntityTypeConfiguration<NetworkHost>
    {
        public void Configure(EntityTypeBuilder<NetworkHost> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.Property(c => c.Id).HasValueGenerator<UUIDv5ValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(NetworkHost.IP)).IsUnique();
        }
    }
}
