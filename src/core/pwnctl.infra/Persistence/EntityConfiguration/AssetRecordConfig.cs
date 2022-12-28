using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Assets.Aggregates;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class AssetRecordConfig : IEntityTypeConfiguration<AssetRecord>
    {
        public void Configure(EntityTypeBuilder<AssetRecord> builder)
        {
            builder.Property(r => r.Id).HasValueGenerator<HashIdValueGenerator>();
            
            builder.HasKey(r => r.Id);

            builder.OwnsOne(r => r.SubjectClass).Property(s => s.Class).IsRequired();

            builder.HasOne(r => r.Domain).WithMany().HasForeignKey(r => r.DomainId).IsRequired(false);
            builder.HasOne(r => r.Host).WithMany().HasForeignKey(r => r.HostId).IsRequired(false);
            builder.HasOne(r => r.Endpoint).WithMany().HasForeignKey(r => r.EndpointId).IsRequired(false);
            builder.HasOne(r => r.NetRange).WithMany().HasForeignKey(r => r.NetRangeId).IsRequired(false);
            builder.HasOne(r => r.Service).WithMany().HasForeignKey(r => r.ServiceId).IsRequired(false);
            builder.HasOne(r => r.DNSRecord).WithMany().HasForeignKey(r => r.DNSRecordId).IsRequired(false);
            builder.HasOne(r => r.Keyword).WithMany().HasForeignKey(r => r.KeywordId).IsRequired(false);
            builder.HasOne(r => r.Email).WithMany().HasForeignKey(r => r.EmailId).IsRequired(false);
            builder.HasOne(r => r.Parameter).WithMany().HasForeignKey(r => r.ParameterId).IsRequired(false);
            builder.HasOne(r => r.CloudService).WithMany().HasForeignKey(r => r.CloudServiceId).IsRequired(false);
            builder.HasOne(r => r.VirtualHost).WithMany().HasForeignKey(r => r.VirtualHostId).IsRequired(false);

            builder.HasMany(r => r.Tasks);
            builder.HasMany(r => r.Tags);
        }
    }
}
