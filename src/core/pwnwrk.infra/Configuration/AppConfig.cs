using pwnwrk.infra.Aws;

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
        }

        public class ApiSettings
        {
            public string ApiKey { get; set; }
        }

        public class DbSettings
        {
            public DbCredentials Credentials { get; set; } = new DbCredentials();
            public string Endpoint { get; set; }
            public string ConnectionString => $"Host={Endpoint};"
                                           + $"Database={AwsConstants.DatabaseName};"
                                           + $"Username={Credentials.Username};"
                                           + $"Password={Credentials.Password}";
            public class DbCredentials
            {
                public string Username { get; set; }
                public string Password { get; set; }
            }
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
            public string Provider { get; set; } = "console";
        }
    }
}
