namespace pwnctl.infra.Commands;

using System.Diagnostics;
using System.Text;
using pwnctl.app;
using pwnctl.app.Common.Interfaces;

public class BashCommandExecutor : CommandExecutor
{
    public async Task<(int, StringBuilder, StringBuilder)> ExecuteAsync(string command, StringBuilder? stdin = null, CancellationToken token = default)
    {
        StringBuilder stdout = new();
        StringBuilder stderr = new();

        var process = new Process();
        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.OutputDataReceived += (s, e) => stdout.AppendLine(e.Data);
        process.ErrorDataReceived += (s, e) => stderr.AppendLine(e.Data);

        process.Start();

        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        if (stdin != null)
        {
            stdin = stdin.Replace("'", "'\\''");
            command = "echo '"+ stdin + "' | " + command;
        }

        using (StreamWriter sr = process.StandardInput)
        {
            await sr.WriteLineAsync(command);
            sr.Flush();
            sr.Close();
        }

        await process.WaitForExitAsync(token);

        return (process.ExitCode, stdout, stderr);
    }
}