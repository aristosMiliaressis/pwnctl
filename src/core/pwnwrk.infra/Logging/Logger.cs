using pwnwrk.infra.Configuration;

namespace pwnwrk.infra.Logging
{
    public class Logger // TODO: make this better
    {
        public static Logger Instance = new();

        public Logger()
        {}

        public void Info(string msg)
        {
            File.AppendAllText(Path.Combine(ConfigurationManager.Config.Logging.LogPath, "pwnctl.log"), $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {msg} \n");
        }
    }    
}