using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnwrk.domain.Entities;

namespace pwnwrk.infra.Persistence.EntityConfiguration
{
    public class TaskConfig : IEntityTypeConfiguration<domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<domain.Entities.Task> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.Definition)
                .WithMany()
                .HasForeignKey(t => t.DefinitionId);

            builder.HasOne(t => t.Host)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.HostId);

            builder.HasOne(t => t.Service)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.ServiceId);

            builder.HasOne(t => t.Endpoint)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.EndpointId);

            builder.HasOne(t => t.Domain)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.DomainId);

            builder.HasOne(t => t.NetRange)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.NetRangeId);

            builder.HasOne(t => t.DNSRecord)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.DNSRecordId);

            builder.HasOne(t => t.CloudService)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.CloudServiceId);

            builder.HasOne(t => t.Keyword)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.KeywordId);
        }
    }

    public class TaskDefinitionConfig : IEntityTypeConfiguration<TaskDefinition>
    {
        public void Configure(EntityTypeBuilder<TaskDefinition> builder)
        {
            builder.HasKey(d => d.Id);
        }
    }
}