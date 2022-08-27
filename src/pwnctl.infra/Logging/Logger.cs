namespace pwnctl.infra.Logging
{
    public class Logger
    {
        public static Logger Instance = new();

        public Logger()
        {}

        public void Info(string msg)
        {
            File.AppendAllText("./pwnctl.log", $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {msg} \n");
        }
    }    
}