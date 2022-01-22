using CommandLine;

namespace pwnctl
{
    public class CommandLineOptions
    {
        [Option(shortName: 'q', longName: "query", Required = false, HelpText = "Query mode (reads SQL from stdin executes and prints output to stdout)")]
        public bool QueryMode { get; set; }

        [Option(shortName: 'p', longName: "process", Required = false, HelpText = "Asset processing mode (reads assets from stdin)")]
        public bool ProcessMode { get; set; }
    }
}