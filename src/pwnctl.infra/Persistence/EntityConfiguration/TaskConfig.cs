using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace pwnctl.infra.Persistence.EntityConfiguration
{
    public class TaskConfig : IEntityTypeConfiguration<core.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<core.Entities.Task> builder)
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

            builder.HasOne(t => t.Keyword)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.KeywordId);
        }
    }

    public class TaskDefinitionConfig : IEntityTypeConfiguration<core.Entities.TaskDefinition>
    {
        public void Configure(EntityTypeBuilder<core.Entities.TaskDefinition> builder)
        {
            builder.HasKey(d => d.Id);
        }
    }
}