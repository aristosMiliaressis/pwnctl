using System.Diagnostics;
using pwnwrk.domain.Tasks.Entities;

namespace pwnwrk.infra.Queues
{
    public sealed class MockJobQueueService : IJobQueueService
    {
        /// <summary>
        /// pushes a job to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public Task EnqueueAsync(TaskRecord job)
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
