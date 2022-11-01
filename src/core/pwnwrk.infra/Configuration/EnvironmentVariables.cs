namespace pwnwrk.infra.Configuration;

public static class EnvironmentVariables
{
    public static string InstallPath => Environment.GetEnvironmentVariable("PWNCTL_InstallPath");
    public static string EfsMountPoint => Environment.GetEnvironmentVariable("PWNCTL_EFS_MOUNT_POINT");
    public static bool InVpc => Environment.GetEnvironmentVariable("PWNCTL_Aws__InVpc")?.Equals("true") ?? false;
}