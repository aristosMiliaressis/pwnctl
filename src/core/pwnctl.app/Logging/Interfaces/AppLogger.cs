namespace pwnctl.app.Logging.Interfaces;

public interface AppLogger
{
    void Exception(Exception ex);

    void Debug(string messageTemplate, params string[] args);

    void Information(string messageTemplate, params string[] args);

    void Warning(string messageTemplate, params string[] args);

    void Error(string messageTemplate, params string[] args);

    void Fatal(string messageTemplate, params string[] args);
}