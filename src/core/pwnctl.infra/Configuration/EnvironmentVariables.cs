namespace pwnctl.infra.Configuration;

public static class EnvironmentVariables
{
    public static string HOSTNAME => Environment.GetEnvironmentVariable("HOSTNAME");
    public static string INSTALL_PATH => Environment.GetEnvironmentVariable("PWNCTL_INSTALL_PATH") ?? "/etc/pwnctl";
    public static bool InVpc => Environment.GetEnvironmentVariable("PWNCTL_Aws__InVpc")?.Equals("true") ?? false;
    public static bool TEST_RUN => Environment.GetEnvironmentVariable("PWNCTL_TEST_RUN")?.Equals("true") ?? false;
}