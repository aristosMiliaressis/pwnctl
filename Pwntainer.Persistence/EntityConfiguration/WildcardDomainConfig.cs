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
    public class WildcardDomainConfig : IEntityTypeConfiguration<WildcardDomain>
    {
        public void Configure(EntityTypeBuilder<WildcardDomain> builder)
        {
            builder.HasKey(e => e.Pattern);
        }
    }
}
