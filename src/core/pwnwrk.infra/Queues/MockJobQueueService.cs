using System;
using System.Diagnostics;
using pwnwrk.domain.Interfaces;
using pwnwrk.infra.Logging;

namespace pwnwrk.infra.Queues
{
    public class MockJobQueueService : IJobQueueService
    {
        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public Task EnqueueAsync(domain.Entities.Task job)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/bash";
            psi.Arguments = $"-c \"echo {job.Command} > mock-queue.txt";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);

            process.WaitForExit();

            return Task.CompletedTask;
        }
    }
}
