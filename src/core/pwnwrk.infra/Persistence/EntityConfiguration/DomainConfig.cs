﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.infra.Persistence.IdGenerators;

namespace pwnwrk.infra.Persistence.EntityConfiguration
{
    public class DomainConfig : IEntityTypeConfiguration<domain.Entities.Assets.Domain>
    {
        public void Configure(EntityTypeBuilder<domain.Entities.Assets.Domain> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(Domain.Name)).IsUnique();
        }
    }
}