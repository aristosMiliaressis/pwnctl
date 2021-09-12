using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Pwntainer.Application.Wrappers.NmapWrapper.NmapOutputEntry;

namespace Pwntainer.Application.Wrappers
{
    /// <summary>
    /// This class handles the invokation of nmap and parsing of stdout into CLR models
    /// don't provide nmap output option.
    /// </summary>
    public static class NmapWrapper
    {
        public static List<NmapOutputEntry> Run(params string[] args)
        {
            var output = new List<NmapOutputEntry>();

            var psi = new ProcessStartInfo
            {
                FileName = "nmap",
                Arguments = string.Join(" ", args),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var proc = new Process
            {
                StartInfo = psi
            };

            proc.Start();

            var stdout = new List<string>();
            var stderr = new List<string>();

            Task.WaitAll(
                Task.Run(() =>
                {
                    while (!proc.StandardOutput.EndOfStream)
                        stdout.Add(proc.StandardOutput.ReadLine());
                }), Task.Run(() =>
                {
                    while (!proc.StandardError.EndOfStream)
                        stderr.Add(proc.StandardError.ReadLine());
                }));

            proc.WaitForExit();

            var lines = stdout.Where(l => l.Contains("/tcp") || l.Contains("/udp"));
            var resultHeaderLine = stdout.FirstOrDefault(l => l.Contains("PORT") && l.Contains("STATE") && l.Contains("SERVICE"));

            foreach (var line in lines)
            {
                var entry = new NmapOutputEntry();

                var segments = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                entry.Port = ushort.Parse(segments[0].Split('/')[0]);
                entry.Protocol = segments[2];
                entry.Status = segments[1] == "open" ? PortStatus.Open :
                               segments[1] == "filtered" ? PortStatus.Filtered :
                               segments[1] == "closed" ? PortStatus.Closed : PortStatus.Open_Or_Filtered;

                if (resultHeaderLine.Contains("VERSION"))
                {
                    entry.Banner = string.Join(" ", segments.Skip(3));
                }

                output.Add(entry);
            }

            return output;
        }

        public class NmapOutputEntry
        {
            public ushort Port { get; set; }
            public PortStatus Status { get; set; }
            public string Protocol { get; set; }
            public string Banner { get; set; }
            public string ScriptOutput { get; set; }

            public enum PortStatus
            {
                Open,
                Closed,
                Filtered,
                Open_Or_Filtered
            }
        }
    }
}
