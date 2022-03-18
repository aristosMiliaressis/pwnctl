using pwnctl;
using pwnctl.Persistence;
using pwnctl.Entities;
using pwnctl.Handlers;
using pwnctl.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pwnctl.Configuration
{
    public static class EnvironmentVariables
    {
        public static string INSTALL_PATH => Environment.GetEnvironmentVariable("INSTALL_PATH");
    }
}
