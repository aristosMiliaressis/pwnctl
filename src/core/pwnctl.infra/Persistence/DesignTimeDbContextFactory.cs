using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using pwnctl.infra.DependencyInjection;

namespace pwnctl.infra.Persistence
{
    public sealed class PwnctlDbContextFactory : IDesignTimeDbContextFactory<PwnctlDbContext>
    {
        public PwnctlDbContext CreateDbContext(string[] args)
        {
            PwnInfraContextInitializer.Setup();

            var optionsBuilder = new DbContextOptionsBuilder<PwnctlDbContext>();
            optionsBuilder.UseNpgsql(PwnctlDbContext.ConnectionString);

            return new PwnctlDbContext(optionsBuilder.Options);
        }
    }
}
