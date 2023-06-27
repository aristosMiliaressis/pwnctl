namespace pwnctl.infra.Commands;

using System.Diagnostics;
using System.Text;
using pwnctl.app;

public static class CommandExecutor
{
    public static async Task<(int, StringBuilder, StringBuilder)> ExecuteAsync(string command, CancellationToken token = default)
    {
        //PwnInfraContext.Logger.Debug("Running: " + command);
        
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