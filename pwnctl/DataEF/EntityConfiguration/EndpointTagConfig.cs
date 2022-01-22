using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwnctl.DataEF.EntityConfiguration
{
    public class EndpointTagConfig : IEntityTypeConfiguration<EndpointTag>
    {
        public void Configure(EntityTypeBuilder<EndpointTag> builder)
        {
            builder.HasKey(e => e.Id);

            builder.OwnsOne(e => e.Tag);

            builder.HasOne(e => e.Endpoint)
                .WithMany()
                .HasForeignKey(e => e.EndpointId);
        }
    }
}
