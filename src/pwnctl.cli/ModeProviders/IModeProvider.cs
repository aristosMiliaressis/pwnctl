using System.Threading.Tasks;

namespace pwnctl.cli.ModeProviders
{
    public interface IModeProvider
    {
        public string ModeName { get; }
        public Task Run(string[] args);
        public void PrintHelpSection();
    }
}