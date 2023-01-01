namespace pwnctl.app.Logging.Interfaces;

public interface AppLogger
{
    public void Exception(Exception ex);

    public void Exception(int sinkBitMap, Exception ex);

    public void Debug(string messageTemplate, params string[] args);

    public void Debug(int sinkBitMap, string messageTemplate, params string[] args);

    public void Information(string messageTemplate, params string[] args);

    public void Information(int sinkBitMap, string messageTemplate, params string[] args);

    public void Warning(string messageTemplate, params string[] args);

    public void Warning(int sinkBitMap, string messageTemplate, params string[] args);

    public void Error(string messageTemplate, params string[] args);

    public void Error(int sinkBitMap, string messageTemplate, params string[] args);

    public void Fatal(string messageTemplate, params string[] args);

    public void Fatal(int sinkBitMap, string messageTemplate, params string[] args);
}