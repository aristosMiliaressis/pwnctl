using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Pwntainer.Application.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pwntainer.Persistence
{
    public class PwntainerDbContext : DbContext
    {
        public static string ConnectionString = "FileName=/opt/pwntainer/pwntainer.db";
        
        public static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
        });

        public PwntainerDbContext()
        {
            Database.Migrate();
        }

        public PwntainerDbContext(DbContextOptions options)
            : this()
        {
        }

        public DbSet<Domain> Domains { get; set; }
        public DbSet<WildcardDomain> WildcardDomains { get; set; }
        public DbSet<NetRange> NetRanges { get; set; }
        public DbSet<Host> Hosts { get; set; }
        public DbSet<VirtualHost> VirtualHosts { get; set; }
        public DbSet<ARecord> ARecords { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceTag> ServiceTags { get; set; }
        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<EndpointTag> EndpointTags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                optionsBuilder.UseLoggerFactory(_loggerFactory).EnableSensitiveDataLogging(true);
#endif
                optionsBuilder.UseSqlite(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
