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
    public class DNSRecordConfig : IEntityTypeConfiguration<DNSRecord>
    {
        public void Configure(EntityTypeBuilder<DNSRecord> builder)
        {
            builder.HasKey(p => new { p.Id });

            builder.HasOne(e => e.Domain)
                .WithMany()
                .HasForeignKey(e => e.DomainId);

            builder.HasOne(e => e.Host)
                .WithMany()
                .HasForeignKey(e => e.HostId);
        }
    }
}
