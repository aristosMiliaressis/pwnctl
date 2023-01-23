using Serilog;
using Serilog.Core;
using Serilog.Events;
using AWS.Logger.SeriLog;
using AWS.Logger;
using pwnctl.infra.Aws;
using pwnctl.infra.Configuration;
using pwnctl.app.Configuration;
using pwnctl.app.Logging.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.app.Logging;

namespace pwnctl.infra.Logging
{
    public static class PwnLoggerFactory
    {
        public static AppLogger Create(AppConfig config, NotificationSender sender)
        {
            var defaultSink = EnvironmentVariables.InVpc ? LogSinks.Console : LogSinks.File;

            return new PwnLogger(defaultSink, sender,
                        fileLogger: CreateFileLogger(config),
                        consoleLogger: CreateConsoleLogger(config));
        }

        private static Logger CreateCloudWatchLogger(AppConfig config)
        {
            var configuration = new AWSLoggerConfig(config.Logging.LogGroup ?? AwsConstants.EcsLogGroupName);

            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(config.Logging.MinLevel))
                    .WriteTo.AWSSeriLog(configuration)
                    .CreateLogger();
        }

        private static Logger CreateFileLogger(AppConfig config)
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(config.Logging.MinLevel))
                    .WriteTo.File(path: Path.Combine(config.Logging.FilePath, "pwnctl.log"),
                                  outputTemplate: _outputTemplate)
                    .CreateLogger();
        }

        private static Logger CreateConsoleLogger(AppConfig config)
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(config.Logging.MinLevel))
                    .WriteTo.Console(outputTemplate: _consoleOutputTemplate)
                    .CreateLogger();
        }

        private static string _outputTemplate = $"[{{Timestamp:yyyy-MM-dd HH:mm:ss.fff}} {EnvironmentVariables.HOSTNAME} {{Level:u3}}] {{Message:lj}}\n";
        private static string _consoleOutputTemplate = $"[{{Level:u3}}] {{Message:lj}}\n";
    }
}