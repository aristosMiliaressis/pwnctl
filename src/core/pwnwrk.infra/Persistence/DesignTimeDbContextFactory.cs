using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using pwnwrk.infra.Configuration;

namespace pwnwrk.infra.Persistence
{
    public class PwnctlDbContextFactory : IDesignTimeDbContextFactory<PwnctlDbContext>
    {
        public PwnctlDbContext CreateDbContext(string[] args)
        {
            var config = PwnConfigFactory.Create();

            var optionsBuilder = new DbContextOptionsBuilder<PwnctlDbContext>();
            optionsBuilder.UseNpgsql(config.Db.ConnectionString);

            return new PwnctlDbContext(optionsBuilder.Options);
        }
    }
}
