using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using System.Text.Json;

namespace pwnctl.infra.Configuration
{
    public static class ConfigurationManager
    {
        public static void Load()
        {
            try
            {
                Directory.CreateDirectory(AppConfig.InstallPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create installation directory {AppConfig.InstallPath}", ex);
            }

            IConfigurationBuilder builder = new ConfigurationBuilder()
                                                    .SetBasePath(Path.GetFullPath(AppConfig.InstallPath))
                                                    .AddIniFile("config.ini", optional: true, reloadOnChange: true)
                                                    .AddEnvironmentVariables(prefix: "PWNCTL_");

            IConfiguration root = builder.Build();
            _config = root.Get<AppConfig>();
        }

        public static AppConfig Config => _config ?? throw new Exception("Configuration hasn't been loaded");
        private static AppConfig _config { get; set; }
    }

    public class AppConfig
    {
        public static string InstallPath = string.IsNullOrWhiteSpace(EnvironmentVariables.PWNCTL_INSTALL_PATH)
                                        ? "/etc/pwnctl/"
                                        : EnvironmentVariables.PWNCTL_INSTALL_PATH;
        public bool IsTestRun { get; set; }

        public DbConfig Db { get; set; } = new DbConfig();
        public JobQueueConfig JobQueue { get; set; } = new JobQueueConfig();

        public class DbConfig
        {
            public string ConnectionString { get; set; }
        }

        public class JobQueueConfig
        {
            public bool IsSQS { get; set; }
            public int WorkerCount { get; set; }
        }
    }
}
