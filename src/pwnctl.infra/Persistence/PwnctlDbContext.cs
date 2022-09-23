using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using pwnctl.infra.Configuration;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Persistence.IdGenerators;

namespace pwnctl.infra.Persistence
{
    public class PwnctlDbContext : DbContext
    {     
        public static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
        });

        public PwnctlDbContext()
        {
        }

        public PwnctlDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<core.Entities.Program> Programs { get; set; }
        public DbSet<core.Entities.ScopeDefinition> ScopeDefinitions { get; set; }
        public DbSet<core.Entities.OperationalPolicy> OperationalPolicies { get; set; }
        public DbSet<core.Entities.Task> Tasks { get; set; }
        public DbSet<core.Entities.TaskDefinition> TaskDefinitions { get; set; }
        public DbSet<core.Entities.Assets.Domain> Domains { get; set; }
        public DbSet<core.Entities.Assets.NetRange> NetRanges { get; set; }
        public DbSet<core.Entities.Assets.Host> Hosts { get; set; }
        public DbSet<core.Entities.Assets.VirtualHost> VirtualHosts { get; set; }
        public DbSet<core.Entities.Assets.DNSRecord> DNSRecords { get; set; }
        public DbSet<core.Entities.Assets.Service> Services { get; set; }
        public DbSet<core.Entities.Assets.Endpoint> Endpoints { get; set; }
        public DbSet<core.Entities.Assets.Parameter> Parameters { get; set; }
        public DbSet<core.Entities.Assets.Email> Emails { get; set; }
        public DbSet<core.Entities.Tag> Tags { get; set; }
        public DbSet<core.Entities.Assets.Keyword> Keywords { get; set; }
        public DbSet<core.Entities.NotificationRule> NotificationRules { get; set; }
        public DbSet<core.Entities.NotificationProviderSettings> NotificationProviderSettings { get; set; }
        public DbSet<core.Entities.NotificationChannel> NotificationChannels { get; set; }
        
        public override int SaveChanges()
        {
            this.ConvertDateTimesToUtc();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.ConvertDateTimesToUtc();
            return base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                optionsBuilder = optionsBuilder.UseLoggerFactory(_loggerFactory).EnableSensitiveDataLogging(true);
#endif
                optionsBuilder = optionsBuilder.ReplaceService<StringValueGenerator, HashIdValueGenerator>();

                if (ConfigurationManager.Config.IsTestRun)
                {
                    optionsBuilder.UseSqlite(ConfigurationManager.Config.Db.ConnectionString, x => x.MigrationsHistoryTable("__EFMigrationHistory"));
                    return;
                }

                optionsBuilder.UseNpgsql(ConfigurationManager.Config.Db.ConnectionString, x => x.MigrationsHistoryTable("__EFMigrationHistory"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
   }
}
