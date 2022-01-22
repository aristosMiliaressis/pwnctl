using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwnctl.Dto
{
    public class UrlDto
    {
        public UrlScheme Scheme { get; set; }
        public string Ip { get; set; }
        public string Domain { get; set; }
        public ushort Port { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> Query { get; set; }

        public enum UrlScheme
        {
            http,
            https,
            ftp
        }
    }
}
