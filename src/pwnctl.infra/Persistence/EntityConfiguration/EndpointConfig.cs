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
    public class EndpointConfig : IEntityTypeConfiguration<Endpoint>
    {
        public void Configure(EntityTypeBuilder<Endpoint> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId);
        }
    }
}
