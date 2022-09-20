using pwnctl.infra.Configuration;

namespace pwnctl.infra.Logging
{
    public class Logger // TODO: make this better
    {
        public static Logger Instance = new();

        public Logger()
        {}

        public void Info(string msg)
        {
            File.AppendAllText($"{AppConfig.InstallPath}/pwnctl.log", $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {msg} \n");
        }
    }    
}