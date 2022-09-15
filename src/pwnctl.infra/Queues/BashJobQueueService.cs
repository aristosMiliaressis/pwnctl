using System.Diagnostics;
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
                var command = @$"{job.Command} | while read assetLine;
                do 
                    if [[ ${{assetLine::1}} == '{{' ]]; 
                    then 
                        echo $assetLine | jq -c '.tags += {{""FoundBy"": ""{job.Definition.ShortName}""}}';
                    else 
                        echo '{{""asset"":""'$assetLine'"", ""tags"":{{""FoundBy"":""{job.Definition.ShortName}""}}}}'; 
                    fi; 
                done | pwnctl process".Replace("\r\n", "");

                sr.WriteLine(command);
                sr.Flush();
                sr.Close();
            }

            process.WaitForExit();
        }
    }
}
