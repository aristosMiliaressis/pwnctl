using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class HttpHostConfig : IEntityTypeConfiguration<HttpHost>
    {
        public void Configure(EntityTypeBuilder<HttpHost> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Socket)
                .WithMany()
                .HasForeignKey(e => e.ServiceId);
        }
    }
}
