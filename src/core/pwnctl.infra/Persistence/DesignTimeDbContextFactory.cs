using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using pwnctl.infra.DependencyInjection;

namespace pwnctl.infra.Persistence
{
    public sealed class PwnctlDbContextFactory : IDesignTimeDbContextFactory<PwnctlDbContext>
    {
        public PwnctlDbContext CreateDbContext(string[] args)
        {
            Environment.SetEnvironmentVariable("PWNCTL_Logging__MinLevel", "Warning");
            PwnInfraContextInitializer.Setup();

            var optionsBuilder = new DbContextOptionsBuilder<PwnctlDbContext>();
            optionsBuilder.UseNpgsql(PwnctlDbContext.ConnectionString);

            return new PwnctlDbContext(optionsBuilder.Options);
        }
    }
}
