using System.Diagnostics;
using pwnctl.infra.Configuration;
using pwnctl.infra.Logging;
using pwnctl.core.Interfaces;

namespace pwnctl.infra.Queues
{
    public class BashJobQueueService : IJobQueueService
    {
        private static readonly string _queueDirectory = "/opt/pwntainer/jobs/";

        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public void Enqueue(core.Entities.Task job)
        {
            Logger.Instance.Info("Enqueue( " + job.Command +")" );

            var psi = new ProcessStartInfo();
            psi.FileName = "job-queue.sh";
            psi.Arguments = $"-w {EnvironmentVariables.BASH_WORKERS} -q {_queueDirectory}";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
            using (StreamWriter sr = process.StandardInput)
            {
                sr.WriteLine(job.Command);
                sr.Flush();
                sr.Close();
            }

            process.WaitForExit();
        }
    }
}
