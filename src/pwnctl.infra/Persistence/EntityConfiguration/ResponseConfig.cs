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
    public class ResponseConfig : IEntityTypeConfiguration<Response>
    {
        public void Configure(EntityTypeBuilder<Response> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.Request)
                .WithMany()
                .HasForeignKey(r => r.RequestId);
        }
    }
}
