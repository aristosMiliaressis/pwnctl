using pwnctl.domain.BaseClasses;
using pwnctl.app.Notifications.Entities;
using pwnctl.app.Notifications.Interfaces;
using System.Diagnostics;

namespace pwnctl.infra.Notifications
{
    public sealed class DiscordNotificationSender : NotificationSender
    {
        public void Send(Asset asset, NotificationRule rule)
        {
            Send($"{asset.DomainIdentifier} triggered rule {rule.ShortName}", rule.Topic);
        }

        public void Send(string message, string topic)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "/root/go/bin/notify";
            psi.Arguments = $"-bulk -provider discord -id {topic}";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);
            using (StreamWriter sr = process.StandardInput)
            {
                sr.WriteLine(message);
                sr.Flush();
                sr.Close();
            }

            process.WaitForExit();
        }
    }
}
