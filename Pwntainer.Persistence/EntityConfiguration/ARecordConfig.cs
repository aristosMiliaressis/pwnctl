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
    public class ARecordConfig : IEntityTypeConfiguration<ARecord>
    {
        public void Configure(EntityTypeBuilder<ARecord> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Domain)
                .WithMany()
                .HasForeignKey(e => e.DomainName);

            builder.HasOne(e => e.Host)
                .WithMany()
                .HasForeignKey(e => e.IP);
        }
    }
}
