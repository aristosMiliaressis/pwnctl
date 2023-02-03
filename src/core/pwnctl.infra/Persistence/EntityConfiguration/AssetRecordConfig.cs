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
            builder.ToTable("asset_records");

            builder.Property(r => r.Id).HasValueGenerator<HashIdValueGenerator>();
            
            builder.HasKey(r => r.Id);

            builder.OwnsOne(r => r.SubjectClass).Property(s => s.Class).IsRequired();

            builder.HasOne(r => r.DomainName).WithMany().HasForeignKey(r => r.DomainNameId).IsRequired(false);
            builder.HasOne(r => r.NetworkHost).WithMany().HasForeignKey(r => r.NetworkHostId).IsRequired(false);
            builder.HasOne(r => r.NetworkRange).WithMany().HasForeignKey(r => r.NetworkRangeId).IsRequired(false);
            builder.HasOne(r => r.NetworkSocket).WithMany().HasForeignKey(r => r.NetworkSocketId).IsRequired(false);
            builder.HasOne(r => r.DomainNameRecord).WithMany().HasForeignKey(r => r.DomainNameRecordId).IsRequired(false);
            builder.HasOne(r => r.Email).WithMany().HasForeignKey(r => r.EmailId).IsRequired(false);
            builder.HasOne(r => r.HttpHost).WithMany().HasForeignKey(r => r.HttpHostId).IsRequired(false);
            builder.HasOne(r => r.HttpEndpoint).WithMany().HasForeignKey(r => r.HttpEndpointId).IsRequired(false);
            builder.HasOne(r => r.HttpParameter).WithMany().HasForeignKey(r => r.HttpParameterId).IsRequired(false);

            builder.HasOne(r => r.FoundByTask).WithMany().HasForeignKey(r => r.FoundByTaskId).IsRequired(false);

            builder.HasMany(r => r.Tasks);
            builder.HasMany(r => r.Tags);
        }
    }
}
