using pwnwrk.infra.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using AWS.Logger.SeriLog;
using AWS.Logger;

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
            AWSLoggerConfig configuration = new AWSLoggerConfig(PwnContext.Config.Logging.LogGroup ?? "/aws/ecs/pwnwrk");

            return new LoggerConfiguration()
                .MinimumLevel.Is(Enum.Parse<LogEventLevel>(PwnContext.Config.Logging.MinLevel ?? "Information"))
                .WriteTo.AWSSeriLog(configuration)
                .CreateLogger();
        }

        private static Logger CreateFileLogger()
        {
            return new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(PwnContext.Config.Logging.MinLevel ?? "Information"))
                    .WriteTo.File(path: Path.Combine(AppConfig.InstallPath, "pwnctl.log"))
                    .WriteTo.Console()
                    .CreateLogger();        
        }
    }
}