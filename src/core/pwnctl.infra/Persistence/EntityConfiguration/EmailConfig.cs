using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class EmailConfig : IEntityTypeConfiguration<Email>
    {
        public void Configure(EntityTypeBuilder<Email> builder)
        {
            builder.ToTable("emails");

            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(t => t.Id);

            builder.HasIndex(nameof(Email.Address)).IsUnique();

            builder.HasOne(e => e.DomainName)
                .WithMany()
                .HasForeignKey(e => e.DomainId);
        }
    }
}
