using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.infra.Persistence.IdGenerators;

namespace pwnwrk.infra.Persistence.EntityConfiguration
{
    public class CloudServiceConfig : IEntityTypeConfiguration<CloudService>
    {
        public void Configure(EntityTypeBuilder<CloudService> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(t => t.Id);

            builder.HasIndex(nameof(Email.Domainname)).IsUnique();

            builder.HasOne(e => e.Domain)
                .WithMany()
                .HasForeignKey(e => e.DomainId);
        }
    }
}