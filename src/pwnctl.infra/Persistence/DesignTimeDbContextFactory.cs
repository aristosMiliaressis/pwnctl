using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Persistence
{
    public class PwnctlDbContextFactory : IDesignTimeDbContextFactory<PwnctlDbContext>
    {
        public PwnctlDbContext CreateDbContext(string[] args)
        {
            ConfigurationManager.Load();

            var optionsBuilder = new DbContextOptionsBuilder<PwnctlDbContext>();
            optionsBuilder.UseNpgsql(ConfigurationManager.Config.Db.ConnectionString);

            return new PwnctlDbContext(optionsBuilder.Options);
        }
    }
}
