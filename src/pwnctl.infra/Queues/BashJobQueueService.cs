﻿using System.Diagnostics;
using pwnctl.infra.Configuration;
using pwnctl.infra.Logging;
using pwnctl.core.Interfaces;
using System.Text;

namespace pwnctl.infra.Queues
{
    public class BashJobQueueService : IJobQueueService
    {
        private static readonly string _queueDirectory = Path.Combine(EnvironmentVariables.PWNCTL_INSTALL_PATH , "jobs/");

        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public void Enqueue(core.Entities.Task job)
        {
            Logger.Instance.Info("Enqueue( " + job.Command +")" );

            var psi = new ProcessStartInfo();
            psi.FileName = "job-queue.sh";
            psi.Arguments = $"-w {EnvironmentVariables.PWNCTL_BASH_WORKERS} -q {_queueDirectory}";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
            using (StreamWriter sr = process.StandardInput)
            { 
                sr.WriteLine($"{job.Command} | while read asset; do printf \"$asset\\\\x{(int)EnvironmentVariables.PWNCTL_DELIMITER:X2}FoundBy:{job.Definition.ShortName}\\\\n\"; done | pwnctl process");
                sr.Flush();
                sr.Close();
            }

            process.WaitForExit();
        }
    }
}
