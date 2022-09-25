using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.core.Entities.Assets;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
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
