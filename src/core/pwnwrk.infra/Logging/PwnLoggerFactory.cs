using pwnwrk.infra.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;

namespace pwnwrk.infra.Logging
{
    public static class PwnLoggerFactory
    {
        private static LoggerConfiguration _loggerConfig => new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(ConfigurationManager.Config.Logging.MinLevel ?? "Information"))
                    .WriteTo.Console()
                    .WriteTo.AmazonCloudWatch(
                        logGroup: ConfigurationManager.Config.Logging.LogGroup ?? "/aws/ecs/pwnctl",
                        logStreamPrefix: DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        logGroupRetentionPolicy: Enum.Parse<LogGroupRetentionPolicy>(ConfigurationManager.Config.Logging.RetentionPeriod ?? "OneWeek")
                    );

        public static Logger Create()
        {
            return _loggerConfig.CreateLogger();
        }
    }
}