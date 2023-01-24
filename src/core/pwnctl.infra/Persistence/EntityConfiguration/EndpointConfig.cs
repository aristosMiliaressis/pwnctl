﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class EndpointConfig : IEntityTypeConfiguration<Endpoint>
    {
        public void Configure(EntityTypeBuilder<Endpoint> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Service)
                .WithMany(s => s.Endpoints)
                .HasForeignKey(e => e.ServiceId);

            builder.HasIndex(nameof(Endpoint.Url)).IsUnique();

            builder.HasOne(e => e.ParentEndpoint)
                .WithMany()
                .HasForeignKey(e => e.ParentEndpointId);
        }
    }
}