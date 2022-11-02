using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.infra.Persistence.IdGenerators;

namespace pwnwrk.infra.Persistence.EntityConfiguration
{
    public sealed class ServiceConfig : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Host)
                .WithMany()
                .HasForeignKey(e => e.HostId);

            builder.HasIndex(nameof(Service.Origin)).IsUnique();
        }
    }
}
