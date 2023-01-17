﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class ServiceConfig : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Host)
                .WithMany(h => h.Services)
                .HasForeignKey(e => e.HostId);

            builder.HasOne(e => e.Domain)
                .WithMany(h => h.Services)
                .HasForeignKey(e => e.DomainId);

            builder.HasIndex(nameof(Service.Origin)).IsUnique();
        }
    }
}
