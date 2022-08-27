
namespace pwnctl.infra.Configuration
{
    public static class EnvironmentVariables
    {
        public static string INSTALL_PATH => Environment.GetEnvironmentVariable("INSTALL_PATH");
        public static bool PWNTAINER_SQS => Environment.GetEnvironmentVariable("PWNTAINER_SQS")?.Equals("true") ?? false;
        public static bool PWNTAINER_TEST => Environment.GetEnvironmentVariable("PWNTAINER_TEST")?.Equals("true") ?? false;
    }
}
