using pwnwrk.infra.Aws;
using pwnwrk.infra.Logging;

namespace pwnwrk.infra.Configuration
{
    public class AppConfig
    {
        public string InstallPath { get; set; }
        public bool IsTestRun { get; set; }
        public DbSettings Db { get; set; } = new DbSettings();
        public JobQueueSettings JobQueue { get; set; } = new JobQueueSettings();
        public LogSettings Logging { get; set; } = new LogSettings();
        public AwsSettings Aws { get; set; } = new AwsSettings();
        public ApiSettings Api { get; set; } = new ApiSettings();

        public class AwsSettings
        {
            public string Profile { get; set; } = "default";
            public bool InVpc { get; set; }
        }

        public class ApiSettings
        {
            public string BaseUrl { get; set; }
            public string ApiKey { get; set; }
        }

        public class DbSettings
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Host { get; set; }
            public string ConnectionString => $"Host={Host};"
                                           + $"Database={AwsConstants.DatabaseName};"
                                           + $"Username={Username};"
                                           + $"Password={Password}";
        }

        public class JobQueueSettings
        {
            public bool UseBash { get; set; }
            public int WorkerCount { get; set; }
        }

        public class LogSettings
        {
            public string MinLevel { get; set; }
            public string LogGroup { get; set; }
            public string FilePath { get; set; }
            public LogProfile Provider { get; set; } = LogProfile.Console;
        }
    }
}
