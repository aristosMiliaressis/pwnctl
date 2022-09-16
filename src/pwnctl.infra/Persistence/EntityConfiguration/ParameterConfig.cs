using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.core.Entities.Assets;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public class ParameterConfig : IEntityTypeConfiguration<Parameter>
    {
        public void Configure(EntityTypeBuilder<Parameter> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Endpoint)
                .WithMany()
                .HasForeignKey(p => p.EndpointId);

            builder.HasIndex(nameof(Parameter.Url), nameof(Parameter.Name), nameof(Parameter.Type)).IsUnique();
        }
    }
}
