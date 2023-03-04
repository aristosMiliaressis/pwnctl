namespace pwnctl.infra.Commands;

using System.Diagnostics;
using pwnctl.app;

public static class CommandExecutor
{
    public static async Task<Process> ExecuteAsync(string command, bool waitProcess = true, CancellationToken token = default)
    {
        PwnInfraContext.Logger.Debug($"Running: {command}");

        var psi = new ProcessStartInfo();
        psi.FileName = "/bin/bash";
        psi.RedirectStandardError = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardInput = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        var process = Process.Start(psi);
        if (process == null)
            throw new Exception("bash process failed to start");

        using (StreamWriter sr = process.StandardInput)
        {
            await sr.WriteLineAsync(command);
            sr.Flush();
            sr.Close();
        }

        if (waitProcess)
            await process.WaitForExitAsync(token);

        return process;
    }
}