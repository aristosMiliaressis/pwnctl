namespace pwnctl.infra.Configuration;

public static class EnvironmentVariables
{
    public static string HOSTNAME => Environment.GetEnvironmentVariable("HOSTNAME") ?? "";
    public static string IMAGE_HASH => Environment.GetEnvironmentVariable("PWNCTL_IMAGE_HASH")?.Split(":")?.Last()?.Substring(0,12) ?? string.Empty;
    
    public static string INSTALL_PATH => Environment.GetEnvironmentVariable("PWNCTL_INSTALL_PATH") ?? "";
  
    public static bool IN_GHA => Environment.GetEnvironmentVariable("GITHUB_ACTIONS")?.Equals("true") ?? false;
    public static bool IN_VPC => Environment.GetEnvironmentVariable("PWNCTL_IN_VPC")?.Equals("true") ?? false;

    public static bool USE_LOCAL_INTEGRATIONS => Environment.GetEnvironmentVariable("PWNCTL_USE_LOCAL_INTEGRATIONS")?.Equals("true") ?? false;
}
