using System.Threading.Tasks;
namespace pwnctl.infra.Commands;

using pwnctl.app.Common.Interfaces;
using System.Diagnostics;
using System.Text;

public class StubCommandExecutor : CommandExecutor
{
    public Task<(int, StringBuilder, StringBuilder)> ExecuteAsync(string command, StringBuilder stdin = null, CancellationToken token = default)
    {
        return Task.FromResult<(int, StringBuilder, StringBuilder)>(new(0, new(), new()));
    }
}