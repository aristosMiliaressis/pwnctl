namespace pwnctl.infra.Logging;

using pwnctl.app.Logging;
using pwnctl.app.Logging.Interfaces;
using pwnctl.kernel.Extensions;
using Serilog;
using Serilog.Events;

public class PwnLogger : AppLogger
{
    private ILogger _logger;

    public PwnLogger(ILogger logger)
    {
        _logger = logger;
    }

    public void Exception(Exception ex)
    {
        Log(LogEventLevel.Error, ex.ToRecursiveExInfo());
    }

    public void Debug(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Debug, messageTemplate, args);
    }

    public void Information(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Information, messageTemplate, args);
    }

    public void Warning(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Warning, messageTemplate, args);
    }

    public void Error(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Error, messageTemplate, args);
    }

    public void Fatal(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Fatal, messageTemplate, args);
    }

    private void Log(LogEventLevel level, string messageTemplate, params string[] args)
    {
        string message = args.Any()
                    ? string.Format(messageTemplate, args)
                    : messageTemplate;

        _logger.Write(level, message);
    }
}
