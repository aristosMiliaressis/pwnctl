using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Persistence.IdGenerators;
using pwnctl.app;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Scope.Entities;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Notifications.Entities;
using pwnctl.domain.Entities;
using pwnctl.app.Tagging.Entities;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Users.Entities;
using pwnctl.infra.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace pwnctl.infra.Persistence
{
    public sealed class PwnctlDbContext : IdentityDbContext<User>
    {
        public static string ConnectionString => $"Host={PwnInfraContext.Config.Db.Host};"
                                              + $"Database={PwnInfraContext.Config.Db.Name};"
                                              + $"Username={PwnInfraContext.Config.Db.Username};"
                                              + $"Password={PwnInfraContext.Config.Db.Password};"
                                              + "Include Error Detail=true";

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

        public DbSet<NetworkHost> NetworkHosts { get; set; }
        public DbSet<NetworkRange> NetworkRanges { get; set; }
        public DbSet<NetworkSocket> NetworkSockets { get; set; }
        public DbSet<DomainName> DomainNames { get; set; }
        public DbSet<DomainNameRecord> DomainNameRecords { get; set; }
        public DbSet<HttpHost> HttpHosts { get; set; }
        public DbSet<HttpEndpoint> HttpEndpoints { get; set; }
        public DbSet<HttpParameter> HttpParameters { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Operation> Operations { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<ScopeAggregate> ScopeAggregates { get; set; }
        public DbSet<ScopeDefinitionAggregate> ScopeDefinitionAggregates { get; set; }
        public DbSet<ScopeDefinition> ScopeDefinitions { get; set; }
        public DbSet<PolicyTaskProfile> PolicyTaskProfiles { get; set; }
        public DbSet<TaskProfile> TaskProfiles { get; set; }
        public DbSet<TaskDefinition> TaskDefinitions { get; set; }
        public DbSet<NotificationRule> NotificationRules { get; set; }
        public DbSet<AssetRecord> AssetRecords { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TaskRecord> TaskRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder = optionsBuilder
                                    .UseLoggerFactory(_loggerFactory)
                                    .EnableSensitiveDataLogging(true)
                                    .ReplaceService<GuidValueGenerator, UUIDv5ValueGenerator>();

                if (EnvironmentVariables.USE_LOCAL_INTEGRATIONS)
                {
                    optionsBuilder.UseSqlite("Data Source="+Path.Combine(EnvironmentVariables.INSTALL_PATH, "pwnctl.sqlite3"), x => x.MigrationsHistoryTable("__EFMigrationHistory"));
                    return;
                }

                optionsBuilder.UseNpgsql(ConnectionString, x => x.MigrationsHistoryTable("__EFMigrationHistory")
                                                                 .CommandTimeout(300));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
   }
}
