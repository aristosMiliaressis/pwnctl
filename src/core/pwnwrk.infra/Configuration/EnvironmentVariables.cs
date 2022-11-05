namespace pwnwrk.infra.Configuration;

public static class EnvironmentVariables
{
    public static string InstallPath => Environment.GetEnvironmentVariable("PWNCTL_InstallPath");
    public static bool InVpc => Environment.GetEnvironmentVariable("PWNCTL_Aws__InVpc")?.Equals("true") ?? false;
    public static bool DisableSecurityManager => Environment.GetEnvironmentVariable("PWNCTL_DisableSecurityManager")?.Equals("true") ?? false;
}