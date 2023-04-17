using Serilog;
using Serilog.Core;
using Serilog.Events;
using AWS.Logger.SeriLog;
using AWS.Logger;
using pwnctl.infra.Configuration;
using pwnctl.app.Configuration;
using pwnctl.app.Logging.Interfaces;

namespace pwnctl.infra.Logging
{
    public static class PwnLoggerFactory
    {
        public static AppLogger Create(AppConfig config)
        {
            var logger = EnvironmentVariables.IN_VPC 
                        ? CreateCloudWatchLogger(config)
                        : CreateFileLogger(config);

            return new PwnLogger(logger);
        }

        private static Logger CreateCloudWatchLogger(AppConfig config)
        {
            var configuration = new AWSLoggerConfig(config.Logging.LogGroup);
            configuration.LogStreamNamePrefix = EnvironmentVariables.HOSTNAME;

            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(config.Logging.MinLevel))
                    .WriteTo.AWSSeriLog(configuration)
                    .CreateLogger();
        }

        private static Logger CreateFileLogger(AppConfig config)
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(config.Logging.MinLevel))
                    .WriteTo.Console(LogEventLevel.Warning, _consoleOutputTemplate)
                    .WriteTo.File(path: Path.Combine(config.Logging.FilePath, "pwnctl.log"),
                                  outputTemplate: _outputTemplate)
                    .CreateLogger();
        }

        private static string _outputTemplate = $"[{{Timestamp:yyyy-MM-dd HH:mm:ss.fff}} {EnvironmentVariables.HOSTNAME} {{Level:u3}}] {{Message:lj}}\n";
        private static string _consoleOutputTemplate = "[{Level:u3}] {Message:lj}\n";
    }
}