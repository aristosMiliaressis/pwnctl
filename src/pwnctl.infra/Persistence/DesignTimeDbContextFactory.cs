using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace pwnctl.infra.Persistence
{
    public class PwnctlDbContextFactory : IDesignTimeDbContextFactory<PwnctlDbContext>
    {
        public PwnctlDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PwnctlDbContext>();
            optionsBuilder.UseSqlite(PwnctlDbContext.ConnectionString);

            return new PwnctlDbContext(optionsBuilder.Options);
        }
    }
}
