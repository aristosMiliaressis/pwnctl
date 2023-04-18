﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.domain.Entities;
using pwnctl.infra.Persistence.IdGenerators;
using Humanizer;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class HttpHostConfig : IEntityTypeConfiguration<HttpHost>
    {
        public void Configure(EntityTypeBuilder<HttpHost> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.Property(c => c.Id).HasValueGenerator<UUIDv5ValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Socket)
                .WithMany()
                .HasForeignKey(e => e.ServiceId);

            builder.HasIndex(nameof(HttpHost.Name), nameof(HttpHost.SocketAddress)).IsUnique();
        }
    }
}
