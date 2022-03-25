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
    public class HostConfig : IEntityTypeConfiguration<Host>
    {
        public void Configure(EntityTypeBuilder<Host> builder)
        {
            builder.HasKey(e => e.Id);
        }
    }
}
