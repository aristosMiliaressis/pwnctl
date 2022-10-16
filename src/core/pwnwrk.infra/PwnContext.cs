using pwnwrk.infra.Configuration;
using Serilog.Core;

namespace pwnwrk.infra
{
    public static class PwnContext
    {
        public static AppConfig Config { get; set; }
        public static Logger Logger { get; set; }
    }
}