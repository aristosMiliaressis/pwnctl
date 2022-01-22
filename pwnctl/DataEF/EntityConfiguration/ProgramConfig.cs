using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pwnctl.Entities;

namespace pwnctl.DataEF.EntityConfiguration
{
    public class ProgramConfig : IEntityTypeConfiguration<pwnctl.Entities.Program>
    {
        public void Configure(EntityTypeBuilder<pwnctl.Entities.Program> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasMany(p => p.Scope)
              .WithOne(d => d.Program)
              .HasForeignKey(d => d.ProgramId)
              .IsRequired(false)
              .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
