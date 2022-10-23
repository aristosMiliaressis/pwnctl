using System.Threading.Tasks;

namespace pwnctl.cli.ModeHandlers
{
    public interface IModeHandler
    {
        public string ModeName { get; }
        public Task Handle(string[] args);
        public void PrintHelpSection();
    }
}