namespace pwnctl.app.Configuration;

public sealed class AppConfig
{
    public DbSettings Db { get; set; } = new DbSettings();
    public TaskQueueSettings TaskQueue { get; set; } = new TaskQueueSettings();
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
    }

    public sealed class DbSettings
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
    }

    public sealed class TaskQueueSettings
    {
        public bool UseBash { get; set; }
        public int WorkerCount { get; set; }
    }

    public sealed class LogSettings
    {
        public string MinLevel { get; set; } = "Information";
        public string LogGroup { get; set; }
        public string FilePath { get; set; }
    }
}
