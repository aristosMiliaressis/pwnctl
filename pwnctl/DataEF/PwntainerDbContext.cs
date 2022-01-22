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

namespace pwnctl.DataEF
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

        public async Task RunSQLAsync(string sql)
        {
            // TODO: maybe split sql on ';' semicolon to execute statements separatly
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                var command = new SQLiteCommand(sql, connection);
                try
                {
                    connection.Open();
                    SQLiteDataReader reader = command.ExecuteReader();
                    var row = Serialize(reader);
                    string json = JsonConvert.SerializeObject(row, Formatting.Indented);
                    Console.WriteLine(json);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        
        private IEnumerable<Dictionary<string, object>> Serialize(SQLiteDataReader reader)
        {
            var results = new List<Dictionary<string, object>>();
            var cols = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++) 
                cols.Add(reader.GetName(i));

            while (reader.Read()) 
                results.Add(SerializeRow(cols, reader));

            return results;
        }
        
        private Dictionary<string, object> SerializeRow(IEnumerable<string> cols, SQLiteDataReader reader) {
            var result = new Dictionary<string, object>();
            foreach (var col in cols) 
                result.Add(col, reader[col]);
            return result;
        }
    }
}
