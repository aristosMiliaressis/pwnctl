using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using System.Text.Json;

namespace pwnwrk.infra.Configuration
{
    public class AppConfig
    {
        public static string InstallPath = string.IsNullOrWhiteSpace(EnvironmentVariables.PWNCTL_INSTALL_PATH)
                                        ? "/etc/pwnctl/"
                                        : EnvironmentVariables.PWNCTL_INSTALL_PATH;

        public bool IsTestRun { get; set; }
        public DbConfig Db { get; set; } = new DbConfig();
        public JobQueueConfig JobQueue { get; set; } = new JobQueueConfig();
        public LogConfig Logging { get; set; } = new LogConfig();

        public class DbConfig
        {
            public string ConnectionString { get; set; }
        }

        public class JobQueueConfig
        {
            public bool UseBash { get; set; }
            public int WorkerCount { get; set; }
            public string QueueName { get; set; }
            public string DLQName { get; set; }
            public int VisibilityTimeout { get; set; }
        }

        public class LogConfig
        {
            public string MinLevel { get; set; }
            public string LogGroup { get; set; }
            public string RetentionPeriod { get; set; }
        }
    }
}
