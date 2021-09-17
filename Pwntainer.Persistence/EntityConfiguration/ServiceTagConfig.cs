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
    public class ServiceTagConfig : IEntityTypeConfiguration<ServiceTag>
    {
        public void Configure(EntityTypeBuilder<ServiceTag> builder)
        {
            builder.HasKey(e => e.Id);

            builder.OwnsOne(m => m.Tag);
        }
    }
}
