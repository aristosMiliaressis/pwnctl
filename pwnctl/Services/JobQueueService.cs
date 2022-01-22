using System;
using System.Diagnostics;

namespace pwnctl.Services
{
    public class JobQueueService
    {
        private static readonly string _queueDirectory = "/opt/pwntainer/jobs/";

        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public void Enqueue(string command)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/bash";
            psi.Arguments = $"-c \"echo {command} | job-queue.sh -q {_queueDirectory}\"";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);

            process.WaitForExit();
        }
    }
}
