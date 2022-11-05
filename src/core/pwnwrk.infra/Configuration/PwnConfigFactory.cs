using Microsoft.Extensions.Configuration;

namespace pwnwrk.infra.Configuration
{
    public static class PwnConfigFactory
    {
        public static AppConfig Create()
        {
            var installPath = EnvironmentVariables.InstallPath ?? "/etc/pwnctl";

            IConfigurationBuilder builder = new ConfigurationBuilder()
                                        .SetBasePath(Path.GetFullPath(installPath))
                                        .AddIniFile("config.ini", optional: true, reloadOnChange: true)
                                        .AddEnvironmentVariables(prefix: "PWNCTL_");
            
            if (!EnvironmentVariables.DisableSecurityManager)
            {
                builder = builder.AddSecretsManager(configurator: options => 
                {
                    options.SecretFilter = entry => entry.Name.StartsWith("/aws/secret/pwnctl/");
                    options.KeyGenerator = (entry, s) => s.Replace("/aws/secret/pwnctl/", "").Replace("/", ":");
                });
            }

            if (!EnvironmentVariables.InVpc)
            {
                builder = builder.AddSystemsManager("/pwnctl");
            }

            return builder.Build().Get<AppConfig>();
        }
    }
}
