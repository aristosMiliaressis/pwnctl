using System;
using System.Diagnostics;
using pwnctl.core.Interfaces;
using pwnctl.infra.Logging;

namespace pwnctl.infra.Queues
{
    public class MockJobQueueService : IJobQueueService
    {
        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public void Enqueue(core.Entities.Task job)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/bash";
            psi.Arguments = $"-c \"echo {job.Command} > mock-queue.txt";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);

            process.WaitForExit();
        }
    }
}
