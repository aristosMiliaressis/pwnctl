using System;

namespace pwnctl.Configuration
{
    public static class EnvironmentVariables
    {
        public static string INSTALL_PATH => Environment.GetEnvironmentVariable("INSTALL_PATH");
    }
}
