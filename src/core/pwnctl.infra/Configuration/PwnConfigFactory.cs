using Microsoft.Extensions.Configuration;
using pwnctl.app.Configuration;

namespace pwnctl.infra.Configuration
{
    public static class PwnConfigFactory
    {
        public static AppConfig Create()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                        .SetBasePath(Path.GetFullPath(EnvironmentVariables.INSTALL_PATH))
                                        .AddIniFile("config.ini", optional: true, reloadOnChange: true)
                                        .AddEnvironmentVariables(prefix: "PWNCTL_");
         
            if (!EnvironmentVariables.TEST_RUN)
            {
                builder = builder
                            .AddSystemsManager("/pwnctl")
                            .AddSecretsManager(configurator: options =>
                {
                    options.SecretFilter = entry => entry.Name.StartsWith("/aws/secret/pwnctl/");
                    options.KeyGenerator = (entry, s) => s.Replace("/aws/secret/pwnctl/", "").Replace("/", ":");
                });
            }

            return builder.Build().Get<AppConfig>();
        }
    }
}
