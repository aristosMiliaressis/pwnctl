using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Logging;
using pwnctl.core.BaseClasses;
using pwnctl.core.Entities;
using pwnctl.infra.Notifications;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.DependencyInjection;
using pwnctl.infra.Configuration;
using pwnctl.infra.Persistence.IdGenerators;
using System.Linq.Expressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
        
        public static PwnctlDbContext Initialize()
        {
            PwnctlDbContext instance = new();

            if (ConfigurationManager.Config.IsTestRun)
            {
                instance.Database.EnsureDeleted();
            }

            if (instance.Database.GetPendingMigrations().Any())
            {
                instance.Database.Migrate();
            }

            var taskDefinitionFile = $"{AppConfig.InstallPath}/seed/task-definitions.yml";

            if (!instance.TaskDefinitions.Any() && File.Exists(taskDefinitionFile))
            {
                var taskText = File.ReadAllText(taskDefinitionFile);
                var deserializer = new DeserializerBuilder()
                           .WithNamingConvention(PascalCaseNamingConvention.Instance) 
                           .Build();
                var taskDefinitions = deserializer.Deserialize<List<TaskDefinition>>(taskText);

                instance.TaskDefinitions.AddRange(taskDefinitions);
                instance.SaveChanges();
            }

            if (!instance.Programs.Any())
            {
                Matcher matcher = new();
                matcher.AddInclude("target-*.json");

                foreach (string file in matcher.GetResultsInFullPath($"{AppConfig.InstallPath}/seed/"))
                {
                    var program = JsonConvert.DeserializeObject<Program>(File.ReadAllText(file));
                    instance.ScopeDefinitions.AddRange(program.Scope);
                    instance.OperationalPolicies.Add(program.Policy);
                    instance.Programs.Add(program);
                    instance.SaveChanges();
                }
            }

            var notificationRulesFile = $"{AppConfig.InstallPath}/seed/notification-rules.yml";

            if (!instance.NotificationRules.Any())
            {
                var taskText = File.ReadAllText(notificationRulesFile);
                var deserializer = new DeserializerBuilder()
                           .WithNamingConvention(PascalCaseNamingConvention.Instance)
                           .Build();
                var notificationSettings = deserializer.Deserialize<NotificationSettings>(taskText);

                instance.NotificationProviderSettings.AddRange(notificationSettings.Providers);
                instance.NotificationRules.AddRange(notificationSettings.Rules);
                instance.SaveChanges();
            }

            return instance;
        }

        public BaseEntity FirstFromLambda(LambdaExpression lambda)
        {
            var type = lambda.Parameters.First().Type;

            var dbSetMethod = _dbSetMethod.MakeGenericMethod(type);
            var queryableDbSet = dbSetMethod.Invoke(this, null);

            var whereMethod = _whereMethod.MakeGenericMethod(type);

            var filteredQueryable = whereMethod.Invoke(null, new object[] { queryableDbSet, lambda });

            var firstOrDefaultMethod = _firstOrDefaultMethod.MakeGenericMethod(type);
            return (BaseEntity)firstOrDefaultMethod.Invoke(null, new object[] { filteredQueryable });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#if DEBUG
                optionsBuilder.UseLoggerFactory(_loggerFactory).EnableSensitiveDataLogging(true);
#endif
                optionsBuilder.ReplaceService<StringValueGenerator, HashIdValueGenerator>()
                            .UseNpgsql(ConfigurationManager.Config.Db.ConnectionString, x => x.MigrationsHistoryTable("__EFMigrationHistory"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private static MethodInfo _dbSetMethod = typeof(PwnctlDbContext).GetMethod(nameof(PwnctlDbContext.Set), BindingFlags.Public | BindingFlags.Instance, null, Array.Empty<Type>(), null);
        private static MethodInfo _whereMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.Where)).First();
        private static MethodInfo _firstOrDefaultMethod = typeof(Queryable).GetMethods().Where(m => m.Name == nameof(Queryable.FirstOrDefault)).First();
    }
}
