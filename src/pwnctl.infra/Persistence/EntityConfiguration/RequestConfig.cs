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
    public class RequestConfig : IEntityTypeConfiguration<Request>
    {
        public void Configure(EntityTypeBuilder<Request> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasOne(r => r.Endpoint)
                .WithMany()
                .HasForeignKey(r => r.EndpointId);
        }
    }
}
