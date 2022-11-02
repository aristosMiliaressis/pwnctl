using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.infra.Persistence.IdGenerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwnwrk.infra.Persistence.EntityConfiguration
{
    public sealed class HostConfig : IEntityTypeConfiguration<Host>
    {
        public void Configure(EntityTypeBuilder<Host> builder)
        {
            builder.Property(c => c.Id).HasValueGenerator<HashIdValueGenerator>();

            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(Host.IP)).IsUnique();
        }
    }
}
