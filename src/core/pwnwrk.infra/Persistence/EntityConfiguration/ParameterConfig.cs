﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.infra.Persistence.IdGenerators;

namespace pwnwrk.infra.Persistence.EntityConfiguration
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