using System.Diagnostics;
using pwnctl.infra.Configuration;
using pwnctl.infra.Logging;
using pwnctl.core.Interfaces;
using System.Text;

namespace pwnctl.infra.Queues
{
    public class BashJobQueueService : IJobQueueService
    {
        private static readonly string _queueDirectory = Path.Combine(AppConfig.InstallPath , "queue/");

        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public async Task EnqueueAsync(core.Entities.Task job)
        {

            var psi = new ProcessStartInfo();
            psi.FileName = "job-queue.sh";
            psi.Arguments = $"-w {ConfigurationManager.Config.JobQueue.WorkerCount} -q {_queueDirectory}";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
            using (StreamWriter sr = process.StandardInput)
            {
                await sr.WriteLineAsync(job.WrappedCommand);
                sr.Flush();
                sr.Close();
            }

            process.WaitForExit();
        }
    }
}
