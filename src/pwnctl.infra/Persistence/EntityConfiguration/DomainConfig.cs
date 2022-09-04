using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.core.Entities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public class DomainConfig : IEntityTypeConfiguration<core.Entities.Assets.Domain>
    {
        public void Configure(EntityTypeBuilder<core.Entities.Assets.Domain> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(nameof(Domain.Name)).IsUnique();
        }
    }
}
