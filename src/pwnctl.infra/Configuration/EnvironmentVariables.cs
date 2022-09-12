
namespace pwnctl.infra.Configuration
{
    public static class EnvironmentVariables
    {
        public static string PWNCTL_INSTALL_PATH => Environment.GetEnvironmentVariable("PWNCTL_INSTALL_PATH");
        public static string PWNCTL_BASH_WORKERS => Environment.GetEnvironmentVariable("PWNCTL_BASH_WORKERS");
        public static bool PWNCTL_SQS => Environment.GetEnvironmentVariable("PWNCTL_SQS")?.Equals("true") ?? false;
        public static bool PWNCTL_TEST => Environment.GetEnvironmentVariable("PWNCTL_TEST")?.Equals("true") ?? false;
    }
}
