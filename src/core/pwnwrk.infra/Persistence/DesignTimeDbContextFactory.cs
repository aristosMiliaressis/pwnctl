using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using pwnwrk.infra.Configuration;

namespace pwnwrk.infra.Persistence
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
