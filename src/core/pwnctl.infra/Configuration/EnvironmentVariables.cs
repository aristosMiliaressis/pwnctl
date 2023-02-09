namespace pwnctl.infra.Configuration;

public static class EnvironmentVariables
{
    public static string HOSTNAME => Environment.GetEnvironmentVariable("HOSTNAME");
    public static string InstallPath => Environment.GetEnvironmentVariable("PWNCTL_InstallPath") ?? "/etc/pwnctl";
    public static bool InVpc => Environment.GetEnvironmentVariable("PWNCTL_Aws__InVpc")?.Equals("true") ?? false;
    public static bool IsTestRun => Environment.GetEnvironmentVariable("PWNCTL_IsTestRun")?.Equals("true") ?? false;
}