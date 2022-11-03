using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Notifications.Entities;
using System.Diagnostics;

namespace pwnwrk.infra.Notifications
{
    public sealed class NotificationSender
    {
        public void Send(Asset asset, NotificationRule rule)
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
