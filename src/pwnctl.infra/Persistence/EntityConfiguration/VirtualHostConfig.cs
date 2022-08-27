using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.core.Entities.Assets;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public class VirtualHostConfig : IEntityTypeConfiguration<VirtualHost>
    {
        public void Configure(EntityTypeBuilder<VirtualHost> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId);
        }
    }
    
}
