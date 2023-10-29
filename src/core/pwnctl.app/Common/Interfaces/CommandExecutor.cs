namespace pwnctl.app.Common.Interfaces;

using System.Diagnostics;
using System.Text;

public interface CommandExecutor
{
    Task<(int exitCode, StringBuilder stdout, StringBuilder stderr)> ExecuteAsync(string command, StringBuilder? stdin = null, CancellationToken token = default);
}
