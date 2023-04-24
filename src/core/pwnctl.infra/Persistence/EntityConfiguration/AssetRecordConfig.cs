using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Assets.Aggregates;
using pwnctl.domain.ValueObjects;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class AssetRecordConfig : IEntityTypeConfiguration<AssetRecord>
    {
        public void Configure(EntityTypeBuilder<AssetRecord> builder)
        {
            builder.ToTable(builder.GetType().GenericTypeArguments[0].Name.Underscore().Pluralize());

            builder.Property(r => r.Id).HasValueGenerator<UUIDv5ValueGenerator>();
            
            builder.HasKey(r => r.Id);

            builder.Property(c => c.SubjectClass)
                    .HasConversion(subject => subject.Value, value => AssetClass.Create(value), 
                    new ValueComparer<AssetClass>((l, r) => l == r, v => v.GetHashCode()));

            builder.HasMany(r => r.Tasks);
            builder.HasMany(r => r.Tags);

            builder.HasOne(r => r.Scope).WithMany().HasForeignKey(r => r.ScopeId).IsRequired(false);
            builder.HasOne(r => r.FoundByTask).WithMany().HasForeignKey(r => r.FoundByTaskId).IsRequired(false);

            builder.HasOne(r => r.DomainName).WithMany().HasForeignKey(r => r.DomainNameId).IsRequired(false);
            builder.HasOne(r => r.NetworkHost).WithMany().HasForeignKey(r => r.NetworkHostId).IsRequired(false);
            builder.HasOne(r => r.NetworkRange).WithMany().HasForeignKey(r => r.NetworkRangeId).IsRequired(false);
            builder.HasOne(r => r.NetworkSocket).WithMany().HasForeignKey(r => r.NetworkSocketId).IsRequired(false);
            builder.HasOne(r => r.DomainNameRecord).WithMany().HasForeignKey(r => r.DomainNameRecordId).IsRequired(false);
            builder.HasOne(r => r.Email).WithMany().HasForeignKey(r => r.EmailId).IsRequired(false);
            builder.HasOne(r => r.HttpHost).WithMany().HasForeignKey(r => r.HttpHostId).IsRequired(false);
            builder.HasOne(r => r.HttpEndpoint).WithMany().HasForeignKey(r => r.HttpEndpointId).IsRequired(false);
            builder.HasOne(r => r.HttpParameter).WithMany().HasForeignKey(r => r.HttpParameterId).IsRequired(false);
        }
    }
}
