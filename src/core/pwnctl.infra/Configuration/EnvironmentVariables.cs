namespace pwnctl.infra.Configuration;

public static class EnvironmentVariables
{
    public static string HOSTNAME => Environment.GetEnvironmentVariable("HOSTNAME");
    public static string IMAGE_HASH => Environment.GetEnvironmentVariable("PWNCTL_IMAGE_HASH")?.Split(":")?.Last()?.Substring(0,12) ?? string.Empty;
    public static string INSTALL_PATH => Environment.GetEnvironmentVariable("PWNCTL_INSTALL_PATH") ?? "/etc/pwnctl";
    public static bool IS_ECS => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PWNCTL_IS_ECS"));
    public static bool IS_LAMBDA => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME"));
    public static bool TEST_RUN => Environment.GetEnvironmentVariable("PWNCTL_TEST_RUN")?.Equals("true") ?? false;
    public static bool DELETE_DB => Environment.GetEnvironmentVariable("PWNCTL_DELETE_DB")?.Equals("true") ?? false;
    public static bool GITHUB_ACTIONS => Environment.GetEnvironmentVariable("GITHUB_ACTIONS")?.Equals("true") ?? false;
}
