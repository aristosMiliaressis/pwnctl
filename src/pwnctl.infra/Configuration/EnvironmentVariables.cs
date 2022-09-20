
namespace pwnctl.infra.Configuration
{
    internal static class EnvironmentVariables
    {
        internal static string PWNCTL_CONNECTION_STRING => Environment.GetEnvironmentVariable("PWNCTL_CONNECTION_STRING");
        internal static string PWNCTL_INSTALL_PATH => Environment.GetEnvironmentVariable("PWNCTL_INSTALL_PATH");
        internal static string PWNCTL_WORKER_COUNT => Environment.GetEnvironmentVariable("PWNCTL_WORKER_COUNT");
        internal static string PWNCTL_SQS => Environment.GetEnvironmentVariable("PWNCTL_SQS");
        internal static string PWNCTL_TEST => Environment.GetEnvironmentVariable("PWNCTL_TEST");
    }
}
