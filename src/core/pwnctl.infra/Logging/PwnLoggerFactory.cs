using Serilog;
using Serilog.Core;
using Serilog.Events;
using pwnctl.infra.Configuration;
using pwnctl.app.Configuration;
using pwnctl.app.Logging.Interfaces;

namespace pwnctl.infra.Logging
{
    public static class PwnLoggerFactory
    {
        public static AppLogger Create(AppConfig config)
        {
            Logger logger = null;

            if (EnvironmentVariables.IS_LAMBDA)
                logger = CreateConsoleLogger(config);
            else if (EnvironmentVariables.IS_ECS || EnvironmentVariables.TEST_RUN)
                logger = CreateConsoleLogger(config);
            else
                logger = CreateFileLogger(config);

            return new PwnLogger(logger);
        }

        private static Logger CreateFileLogger(AppConfig config)
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(config.Logging.MinLevel))
                    .WriteTo.Console(LogEventLevel.Warning, _consoleOutputTemplate)
                    .WriteTo.File(path: Path.Combine(config.Logging.FilePath, "pwnctl.log"),
                                  outputTemplate: _fileOutputTemplate)
                    .CreateLogger();
        }

        private static Logger CreateConsoleLogger(AppConfig config)
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(config.Logging.MinLevel))
                    .WriteTo.Console(outputTemplate: _consoleOutputTemplate)
                    .CreateLogger();
        }

        private static string _fileOutputTemplate = $"[{{Timestamp:yyyy-MM-dd HH:mm:ss.fff}} {EnvironmentVariables.HOSTNAME} {{Level:u3}}] {{Message:lj}}\n";
        private static string _consoleOutputTemplate = "[{Level:u3}] {Message:lj}\n";
    }
}
