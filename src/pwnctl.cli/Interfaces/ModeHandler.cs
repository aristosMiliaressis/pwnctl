using System.Threading.Tasks;

namespace pwnctl.cli.Interfaces
{
    public interface ModeHandler
    {
        public string ModeName { get; }
        public Task Handle(string[] args);
        public void PrintHelpSection();
    }
}