namespace pwnctl.infra.Commands;

using System.Diagnostics;
using pwnctl.app;

public static class CommandExecutor
{
    public static async Task<Process> ExecuteAsync(string fileName, string args, string stdin = null, CancellationToken token = default)
    {
        PwnInfraContext.Logger.Debug($"Running: {fileName} {args} " + (stdin == null ? "" : $"<<< {stdin}"));

        var psi = new ProcessStartInfo();
        psi.FileName = fileName;
        psi.Arguments = args;
        psi.RedirectStandardError = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardInput = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        var process = Process.Start(psi);
        if (process == null)
            throw new Exception(fileName + " process failed to start");

        if (stdin != null)
        {
            using (StreamWriter sr = process.StandardInput)
            {
                await sr.WriteLineAsync(stdin);
                sr.Flush();
                sr.Close();
            }
        }
        
        await process.WaitForExitAsync(token);

        return process;
    }
}