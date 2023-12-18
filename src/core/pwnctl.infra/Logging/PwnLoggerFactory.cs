namespace pwnctl.infra.Logging;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using pwnctl.infra.Configuration;
using pwnctl.app.Configuration;
using pwnctl.app.Logging.Interfaces;

public static class PwnLoggerFactory
{
    private static string _fileOutputTemplate = $"[{{Timestamp:yyyy-MM-dd HH:mm:ss.fff}} {EnvironmentVariables.COMMIT_HASH} {EnvironmentVariables.HOSTNAME} {{Level:u3}}] {{Message:lj}}\n";
    private static string _consoleOutputTemplate = EnvironmentVariables.COMMIT_HASH + " [{Level:u3}] {Message:lj}\n";

    public static AppLogger DefaultLogger = new PwnLogger(new LoggerConfiguration()
                .MinimumLevel.Is(LogEventLevel.Information)
                .WriteTo.File(path: ".", outputTemplate: _consoleOutputTemplate)
                .CreateLogger());

    public static AppLogger Create(AppConfig config)
    {
        Logger? logger = null;

        if (string.IsNullOrEmpty(config.Logging.FilePath))
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
}
