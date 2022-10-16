using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Entities;
using pwnwrk.infra.Configuration;
using pwnwrk.infra.Logging;
using System.Diagnostics;
using Serilog.Core;

namespace pwnwrk.infra.Notifications
{
    public class NotificationSender
    {
        public void Send(BaseAsset asset, NotificationRule rule)
        {
            PwnContext.Logger.Debug("Send( " + rule.ShortName + ")");

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
