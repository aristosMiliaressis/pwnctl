using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwnctl.DataEF
{
    public class PwntainerDbContextFactory : IDesignTimeDbContextFactory<PwntainerDbContext>
    {
        public PwntainerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PwntainerDbContext>();
            optionsBuilder.UseSqlite(PwntainerDbContext.ConnectionString);

            return new PwntainerDbContext(optionsBuilder.Options);
        }
    }
}
