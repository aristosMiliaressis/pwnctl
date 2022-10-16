using pwnwrk.infra.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;

namespace pwnwrk.infra.Logging
{
    public static class PwnLoggerFactory
    {
        public static Logger Create()
        {
            return PwnContext.Config.IsTestRun
                ? CreateFileLogger()
                : CreateCloudWatchLogger();
        }

        private static Logger CreateCloudWatchLogger()
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(PwnContext.Config.Logging.MinLevel ?? "Information"))
                    .WriteTo.AmazonCloudWatch(
                        logGroup: PwnContext.Config.Logging.LogGroup ?? "/aws/ecs/pwnctl",
                        logStreamPrefix: DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        logGroupRetentionPolicy: Enum.Parse<LogGroupRetentionPolicy>(PwnContext.Config.Logging.RetentionPeriod ?? "OneWeek")
                    ).CreateLogger();
        }

        private static Logger CreateFileLogger()
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(PwnContext.Config.Logging.MinLevel ?? "Information"))
                    .WriteTo.File(
                        path: Path.Combine(EnvironmentVariables.PWNCTL_INSTALL_PATH, "pwnctl.log"), 
                        restrictedToMinimumLevel: Enum.Parse<LogEventLevel>(PwnContext.Config.Logging.MinLevel ?? "Information"))
                    .CreateLogger();        
        }
    }
}