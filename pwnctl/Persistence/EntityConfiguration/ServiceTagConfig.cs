using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwnctl.Persistence.EntityConfiguration
{
    public class ServiceTagConfig : IEntityTypeConfiguration<ServiceTag>
    {
        public void Configure(EntityTypeBuilder<ServiceTag> builder)
        {
            builder.HasKey(e => e.Id);

            builder.OwnsOne(m => m.Tag);
        }
    }
}
