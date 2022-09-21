
namespace pwnctl.infra.Configuration
{
    internal static class EnvironmentVariables
    {
        internal static string PWNCTL_INSTALL_PATH => Environment.GetEnvironmentVariable("PWNCTL_INSTALL_PATH");
    }
}
