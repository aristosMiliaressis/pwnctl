using IniParser;
using IniParser.Model;

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
                throw new Exception($"Unable to create InstallPath {AppConfig.InstallPath}", ex);
            }

            _config = new AppConfig();
            _config.DbConnectionString = $"Data Source={AppConfig.InstallPath}/pwntainer.db";

            if (File.Exists($"{AppConfig.InstallPath}/config.ini"))
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile($"{AppConfig.InstallPath}/config.ini");

                _config.DbConnectionString = data["DB"]["ConnectionString"];
                if (!int.TryParse(data["JobQueue"]["WorkerCount"], out int count))
                    _config.JobQueue.WorkerCount = count;
                if (!string.IsNullOrEmpty(data["JobQueue"]["IsSQS"]))
                    _config.JobQueue.IsSQS = data["JobQueue"]["IsSQS"].ToLower().Equals("true");
            }

            if (!string.IsNullOrEmpty(EnvironmentVariables.PWNCTL_TEST))
                _config.IsTestRun = EnvironmentVariables.PWNCTL_TEST.ToLower().Equals("true");

            if (!string.IsNullOrEmpty(EnvironmentVariables.PWNCTL_SQS))
                _config.JobQueue.IsSQS = EnvironmentVariables.PWNCTL_SQS.ToLower().Equals("true");

            if (!int.TryParse(EnvironmentVariables.PWNCTL_WORKER_COUNT, out int workerCount))
                _config.JobQueue.WorkerCount = workerCount;

            if (!string.IsNullOrEmpty(EnvironmentVariables.PWNCTL_CONNECTION_STRING))
                _config.DbConnectionString = EnvironmentVariables.PWNCTL_CONNECTION_STRING;
        }

        public static AppConfig Config => _config ?? throw new Exception("Configuration hasn't been loaded");
        private static AppConfig _config { get; set; }
    }

    public class AppConfig
    {
        public static string InstallPath = string.IsNullOrWhiteSpace(EnvironmentVariables.PWNCTL_INSTALL_PATH)
                                        ? "/etc/pwnctl"
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
