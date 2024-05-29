namespace pwnctl.infra.Configuration;

public static class EnvironmentVariables
{
    public static string HOSTNAME => Environment.GetEnvironmentVariable("HOSTNAME") ?? "";
    public static string COMMIT_HASH => Environment.GetEnvironmentVariable("PWNCTL_COMMIT_HASH") ?? "";

    public static string FS_MOUNT_POINT => Environment.GetEnvironmentVariable("PWNCTL_FS_MOUNT_POINT") ?? "";

    public static bool IS_GHA => Environment.GetEnvironmentVariable("GITHUB_ACTIONS")?.Equals("true") ?? false;
    public static bool IS_PROD => Environment.GetEnvironmentVariable("PWNCTL_IS_PROD")?.Equals("true") ?? false;

    public static bool USE_LOCAL_INTEGRATIONS => Environment.GetEnvironmentVariable("PWNCTL_USE_LOCAL_INTEGRATIONS")?.Equals("true") ?? false;
}
