using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;

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
                                                    .AddIniFile(Path.Combine(AppConfig.InstallPath,"config.ini"), optional: true, reloadOnChange: true)
                                                    .AddEnvironmentVariables(prefix: "PWNCTL_");

            IConfiguration root = builder.Build();
            _config = root.Get<AppConfig>();
            if (_config.DbConnectionString == null)
                _config.DbConnectionString = $"Data Source={AppConfig.InstallPath}/pwntainer.db";
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
        public string DbConnectionString { get; set; }
        public JobQueueConfig JobQueue { get; set; } = new JobQueueConfig();

        public class JobQueueConfig
        {
            public bool IsSQS { get; set; }
            public int WorkerCount { get; set; }
        }
    }
}
