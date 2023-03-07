using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Persistence.IdGenerators;
using pwnctl.app;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.domain.Entities;
using pwnctl.app.Tagging.Entities;
using pwnctl.app.Assets.Aggregates;
using pwnctl.infra.Aws;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Persistence
{
    public sealed class PwnctlDbContext : DbContext
    {
        public static string ConnectionString => $"Host={PwnInfraContext.Config.Db.Host};"
                                              + $"Database={AwsConstants.DatabaseName};"
                                              + $"Username={AwsConstants.AuroraInstanceUsername};"
                                              + $"Password={PwnInfraContext.Config.Db.Password};Timeout=30";

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

        public DbSet<NetworkHost> Hosts { get; set; }
        public DbSet<NetworkRange> NetworkRanges { get; set; }
        public DbSet<NetworkSocket> Sockets { get; set; }
        public DbSet<DomainName> Domains { get; set; }
        public DbSet<DomainNameRecord> DNSRecords { get; set; }
        public DbSet<HttpHost> HttpHosts { get; set; }
        public DbSet<HttpEndpoint> HttpEndpoints { get; set; }
        public DbSet<HttpParameter> HttpParameters { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<NotificationRule> NotificationRules { get; set; }
        public DbSet<ScopeDefinition> ScopeDefinitions { get; set; }
        public DbSet<OperationalPolicy> OperationalPolicies { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<TaskEntry> TaskEntries { get; set; }
        public DbSet<TaskDefinition> TaskDefinitions { get; set; }
        public DbSet<TaskProfile> TaskProfiles { get; set; }
        public DbSet<AssetRecord> AssetRecords { get; set; }
        public DbSet<Tag> Tags { get; set; }

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
                optionsBuilder = optionsBuilder
                                    .UseLoggerFactory(_loggerFactory)
                                    .EnableSensitiveDataLogging(true)
                                    .ReplaceService<StringValueGenerator, HashIdValueGenerator>();

                if (EnvironmentVariables.TEST_RUN)
                {
                    optionsBuilder.UseSqlite($"Data Source=./pwnctl.db", x => x.MigrationsHistoryTable("__EFMigrationHistory"));
                    return;
                }

                optionsBuilder.UseNpgsql(ConnectionString, x => x.MigrationsHistoryTable("__EFMigrationHistory"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
   }
}
