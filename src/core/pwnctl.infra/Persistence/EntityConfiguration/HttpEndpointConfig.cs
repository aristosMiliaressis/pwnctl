﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;
using Humanizer;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class HttpEndpointConfig : IEntityTypeConfiguration<HttpEndpoint>
    {
        public void Configure(EntityTypeBuilder<HttpEndpoint> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.Property(c => c.Id).HasValueGenerator<UUIDv5ValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Socket)
                .WithMany()
                .HasForeignKey(e => e.SocketAddressId);

            builder.HasIndex(nameof(HttpEndpoint.Url)).IsUnique();

            builder.HasOne(e => e.BaseEndpoint)
                .WithMany()
                .HasForeignKey(e => e.BaseEndpointId);
        }
    }
}
