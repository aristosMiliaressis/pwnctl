using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Persistence
{
    public class PwnctlDbContextFactory : IDesignTimeDbContextFactory<PwnctlDbContext>
    {
        public PwnctlDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PwnctlDbContext>();
            optionsBuilder.UseSqlite($"Data Source={AppConfig.InstallPath}/pwntainer.db");

            return new PwnctlDbContext(optionsBuilder.Options);
        }
    }
}
