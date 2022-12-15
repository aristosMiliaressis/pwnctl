using System.Diagnostics;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Interfaces;

namespace pwnctl.infra.Queues
{
    public sealed class BashTaskQueueService : TaskQueueService
    {
        private static readonly string _queueDirectory = Path.Combine(PwnContext.Config.InstallPath , "queue/");

        /// <summary>
        /// pushes a task to the pending queue.
        /// </summary>
        /// <param name="command"></param>
        public /*async*/ Task EnqueueAsync(TaskRecord task)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "job-queue.sh";
            psi.Arguments = $"-w {PwnContext.Config.TaskQueue.WorkerCount} -q {_queueDirectory}";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
            using (StreamWriter sr = process.StandardInput)
            {
                //await sr.WriteLineAsync(task.WrappedCommand);
                sr.Flush();
                sr.Close();
            }

            process.WaitForExit();

            task.Queued();

            return Task.CompletedTask;
        }
    }
}
