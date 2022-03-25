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
    public class TagConfig : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Host)
                .WithMany()
                .HasForeignKey(t => t.HostId);

            builder.HasOne(t => t.Service)
                .WithMany()
                .HasForeignKey(t => t.ServiceId);

            builder.HasOne(t => t.Endpoint)
                .WithMany()
                .HasForeignKey(t => t.EndpointId);
        }
    }
}
