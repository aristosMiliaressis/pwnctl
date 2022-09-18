using pwnctl.core.BaseClasses;
using pwnctl.core.Entities;
using pwnctl.infra.Configuration;
using pwnctl.infra.Logging;
using System.Diagnostics;

namespace pwnctl.infra.Notifications
{
    public class NotificationSender
    {
        public void Send(BaseAsset asset, NotificationRule rule)
        {
            Logger.Instance.Info("Send( " + rule.ShortName + ")");

            var psi = new ProcessStartInfo();
            psi.FileName = "notify";
            psi.Arguments = $"-bulk -provider discord -id {rule.Topic}";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
            using (StreamWriter sr = process.StandardInput)
            {
                sr.WriteLine($"{asset.DomainIdentifier} triggered rule {rule.ShortName}");
                sr.Flush();
                sr.Close();
            }

            process.WaitForExit();
        }
    }
}
