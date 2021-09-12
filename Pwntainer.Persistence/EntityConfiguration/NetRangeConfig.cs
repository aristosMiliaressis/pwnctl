using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pwntainer.Application.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pwntainer.Persistence.EntityConfiguration
{
    public class NetRangeConfig : IEntityTypeConfiguration<NetRange>
    {
        public void Configure(EntityTypeBuilder<NetRange> builder)
        {
            builder.HasKey(e => e.CIDR);
        }
    }
}
