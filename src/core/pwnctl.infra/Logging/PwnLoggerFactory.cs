using Serilog;
using Serilog.Core;
using Serilog.Events;
using AWS.Logger.SeriLog;
using AWS.Logger;
using pwnctl.infra.Aws;
using pwnctl.infra.Configuration;

namespace pwnctl.infra.Logging
{
    public static class PwnLoggerFactory
    {
        public static Logger Create()
        {
            return PwnContext.Config.Logging.Provider switch
            {
                LogProfile.Console => CreateConsoleLogger(),
                LogProfile.CloudWatch => CreateCloudWatchLogger(),
                LogProfile.File => CreateFileLogger(),
                _ => throw new NotSupportedException()
            };
        }

        private static Logger CreateCloudWatchLogger()
        {
            var configuration = new AWSLoggerConfig(PwnContext.Config.Logging.LogGroup ?? AwsConstants.EcsLogGroupName);

            return _baseConfig
                    .WriteTo.AWSSeriLog(configuration)
                    .CreateLogger();
        }

        private static Logger CreateFileLogger()
        {
            return _baseConfig
                    .WriteTo.File(path: Path.Combine(PwnContext.Config.Logging.FilePath, "pwnctl.log"),
                                  outputTemplate: _outputTemplate)
                    .CreateLogger();
        }

        private static Logger CreateConsoleLogger()
        {
            return _baseConfig
                    .WriteTo.Console()
                    .CreateLogger();
        }

        private static string _outputTemplate = $"[{{Timestamp:HH:mm:ss}} {EnvironmentVariables.HOSTNAME} {{Level:u3}}] {{Message:lj}}\n";

        private static readonly LoggerConfiguration _baseConfig = new LoggerConfiguration()
                    .MinimumLevel.Is(Enum.Parse<LogEventLevel>(PwnContext.Config.Logging.MinLevel ?? "Information"));
    }

    public enum LogProfile
    {
        Console,
        CloudWatch,
        File
    }
}