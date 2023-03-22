using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class DomainNameConfig : IEntityTypeConfiguration<DomainName>
    {
        public void Configure(EntityTypeBuilder<DomainName> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<UUIDv5ValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(DomainName.Name)).IsUnique();

            builder.HasOne(e => e.ParentDomain)
                .WithMany()
                .HasForeignKey(e => e.ParentDomainId);
        }
    }
}
