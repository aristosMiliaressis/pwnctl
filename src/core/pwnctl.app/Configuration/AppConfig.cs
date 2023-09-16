namespace pwnctl.app.Configuration;

public sealed class AppConfig
{
    public DbSettings Db { get; set; } = new DbSettings();
    public WorkerSettings Worker { get; set; } = new WorkerSettings();
    public QueueSettings TaskQueue { get; set; } = new QueueSettings();
    public QueueSettings OutputQueue { get; set; } = new QueueSettings();
    public LogSettings Logging { get; set; } = new LogSettings();
    public AwsSettings Aws { get; set; } = new AwsSettings();
    public ApiSettings Api { get; set; } = new ApiSettings();

    public sealed class AwsSettings
    {
        public string Profile { get; set; } = "default";
    }

    public sealed class ApiSettings
    {
        public string BaseUrl { get; set; }
        public string HMACSecret { get; set; }
        public int AccessTimeoutMinutes { get; set; }
        public int RefreshTimeoutHours { get; set; }
        public int BatchSize { get; set; } = 512;
    }

    public sealed class DbSettings
    {
        public string Name { get; set; } = "pwnctl";
        public string Username { get; set; } = "pwnadmin";
        public string Password { get; set; }
        public string Host { get; set; }
    }

    public sealed class QueueSettings
    {
        public string Name { get; set; }
        public int VisibilityTimeout { get; set; }
    }

    public sealed class WorkerSettings
    {
        public int MaxTaskTimeout { get; set; }
    }

    public sealed class LogSettings
    {
        public string MinLevel { get; set; } = "Information";
        public string LogGroup { get; set; }
        public string FilePath { get; set; }
    }
}
