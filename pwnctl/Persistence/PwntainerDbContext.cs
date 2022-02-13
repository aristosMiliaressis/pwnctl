using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using pwnctl.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pwnctl.Persistence
{
    public class PwntainerDbContext : DbContext
    {
        public static string ConnectionString = "Data Source=/opt/pwntainer/pwntainer.db";
        
        public static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
        });

        public PwntainerDbContext()
        {
            Database.Migrate();
        }

        public PwntainerDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<pwnctl.Entities.Program> Programs { get; set; }
        public DbSet<ScopeDefinition> ScopeDefinitions { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<NetRange> NetRanges { get; set; }
        public DbSet<Host> Hosts { get; set; }
        public DbSet<VirtualHost> VirtualHosts { get; set; }
        public DbSet<DNSRecord> DNSRecords { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Endpoint> Endpoints { get; set; }
        public DbSet<EndpointTag> EndpointTags { get; set; }
        public DbSet<ServiceTag> ServiceTags { get; set; }

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
