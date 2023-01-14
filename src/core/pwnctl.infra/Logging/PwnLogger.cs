using pwnctl.app.Logging;
using pwnctl.app.Logging.Interfaces;
using pwnctl.app.Notifications.Interfaces;
using pwnctl.kernel.Extensions;
using pwnctl.infra.Configuration;
using Serilog;
using Serilog.Events;
using pwnctl.app.Notifications.Enums;

public class PwnLogger : AppLogger
{
    private int _defaultSinkBitMap;
    private ILogger _consoleLogger;
    private ILogger _fileLogger;
    private NotificationSender _notificationSender;

    public PwnLogger(int sinkBitMap, NotificationSender sender, ILogger fileLogger, ILogger consoleLogger)
    {
        _defaultSinkBitMap = sinkBitMap;
        _notificationSender = sender;
        _fileLogger = fileLogger;
        _consoleLogger = consoleLogger;
    }

    public void Exception(Exception ex)
    {
        Log(LogEventLevel.Error, _defaultSinkBitMap, ex.ToRecursiveExInfo());
    }

    public void Exception(int sinkBitMap, Exception ex)
    {
        Log(LogEventLevel.Error, sinkBitMap, ex.ToRecursiveExInfo());
    }

    public void Debug(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Debug, _defaultSinkBitMap, messageTemplate, args);
    }

    public void Debug(int sinkBitMap, string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Debug, sinkBitMap, messageTemplate, args);
    }

    public void Information(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Information, _defaultSinkBitMap, messageTemplate, args);
    }

    public void Information(int sinkBitMap, string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Information, sinkBitMap, messageTemplate, args);
    }

    public void Warning(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Warning, _defaultSinkBitMap, messageTemplate, args);
    }

    public void Warning(int sinkBitMap, string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Warning, sinkBitMap, messageTemplate, args);
    }

    public void Error(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Error, _defaultSinkBitMap, messageTemplate, args);
    }

    public void Error(int sinkBitMap, string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Error, sinkBitMap, messageTemplate, args);
    }

    public void Fatal(string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Fatal, _defaultSinkBitMap, messageTemplate, args);
    }

    public void Fatal(int sinkBitMap, string messageTemplate, params string[] args)
    {
        Log(LogEventLevel.Fatal, sinkBitMap, messageTemplate, args);
    }

    private void Log(LogEventLevel level, int sinkBitMap, string messageTemplate, params string[] args)
    {
        string message = args.Any()
                    ? string.Format(messageTemplate, args)
                    : messageTemplate;

        if ((sinkBitMap & LogSinks.File) > 0)
        {
            _fileLogger.Write(level, message);
        }

        if ((sinkBitMap & LogSinks.Console) > 0)
        {
            _consoleLogger.Write(level, message);
        }

        if ((sinkBitMap & LogSinks.Notification) > 0)
        {
            _notificationSender.Send($"[{EnvironmentVariables.HOSTNAME}] {message}", NotificationTopic.status);
        }
    }
}
