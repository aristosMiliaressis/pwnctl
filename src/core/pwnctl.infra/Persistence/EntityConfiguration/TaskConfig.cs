using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.app.Entities;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public sealed class TaskConfig : IEntityTypeConfiguration<TaskRecord>
    {
        public void Configure(EntityTypeBuilder<TaskRecord> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Definition)
                .WithMany()
                .HasForeignKey(t => t.DefinitionId);

            builder.HasOne(t => t.Host)
                .WithMany()
                .HasForeignKey(t => t.HostId);

            builder.HasOne(t => t.Service)
                .WithMany()
                .HasForeignKey(t => t.ServiceId);

            builder.HasOne(t => t.Endpoint)
                .WithMany()
                .HasForeignKey(t => t.EndpointId);

            builder.HasOne(t => t.Domain)
                .WithMany()
                .HasForeignKey(t => t.DomainId);

            builder.HasOne(t => t.NetRange)
                .WithMany()
                .HasForeignKey(t => t.NetRangeId);

            builder.HasOne(t => t.DNSRecord)
                .WithMany()
                .HasForeignKey(t => t.DNSRecordId);

            builder.HasOne(t => t.CloudService)
                .WithMany()
                .HasForeignKey(t => t.CloudServiceId);

            builder.HasOne(t => t.Keyword)
                .WithMany()
                .HasForeignKey(t => t.KeywordId);
        }
    }

    public sealed class TaskDefinitionConfig : IEntityTypeConfiguration<TaskDefinition>
    {
        public void Configure(EntityTypeBuilder<TaskDefinition> builder)
        {
            builder.HasKey(d => d.Id);
        }
    }
}