﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.infra.Persistence.IdGenerators;

namespace pwnwrk.infra.Persistence.EntityConfiguration
{
    public sealed class NetRangeConfig : IEntityTypeConfiguration<NetRange>
    {
        public void Configure(EntityTypeBuilder<NetRange> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(NetRange.FirstAddress), nameof(NetRange.NetPrefixBits)).IsUnique();
        }
    }
}
