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
    public class NetRangeConfig : IEntityTypeConfiguration<NetRange>
    {
        public void Configure(EntityTypeBuilder<NetRange> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}
