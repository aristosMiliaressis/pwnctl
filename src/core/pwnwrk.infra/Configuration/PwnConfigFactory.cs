using Microsoft.Extensions.Configuration;

namespace pwnwrk.infra.Configuration
{
    public static class PwnConfigFactory
    {
        public static AppConfig Create()
        {
            var installPath = EnvironmentVariables.InstallPath ?? "/etc/pwnctl";

            IConfigurationBuilder builder = new ConfigurationBuilder()
                                        .AddEnvironmentVariables(prefix: "PWNCTL_")
                                        .AddSecretsManager(configurator: options => 
                                        {
                                            options.SecretFilter = entry => entry.Name.StartsWith("/aws/secret/pwnctl/");
                                            options.KeyGenerator = (entry, s) => s.Replace("/aws/secret/pwnctl/", "").Replace("/", ":");
                                        });

            if (!EnvironmentVariables.InVpc)
            {
                builder = builder
                            .SetBasePath(Path.GetFullPath(installPath))
                            .AddIniFile("config.ini", optional: true, reloadOnChange: true)
                            .AddSystemsManager("/pwnctl");
            }

            return builder.Build().Get<AppConfig>();
        }
    }
}
