using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Persistence
{
    public sealed class PwnctlDbContextFactory : IDesignTimeDbContextFactory<PwnctlDbContext>
    {
        public PwnctlDbContext CreateDbContext(string[] args)
        {
            var config = PwnConfigFactory.Create();

            var optionsBuilder = new DbContextOptionsBuilder<PwnctlDbContext>();
            optionsBuilder.UseNpgsql(PwnctlDbContext.ConnectionString);

            return new PwnctlDbContext(optionsBuilder.Options);
        }
    }
}
