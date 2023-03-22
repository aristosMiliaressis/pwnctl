﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class NetworkRangeConfig : IEntityTypeConfiguration<NetworkRange>
    {
        public void Configure(EntityTypeBuilder<NetworkRange> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<UUIDv5ValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(NetworkRange.FirstAddress), nameof(NetworkRange.NetPrefixBits)).IsUnique();
        }
    }
}
