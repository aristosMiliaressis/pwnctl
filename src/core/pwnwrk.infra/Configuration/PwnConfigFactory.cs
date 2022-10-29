using Microsoft.Extensions.Configuration;

using pwnwrk.infra.Aws;

namespace pwnwrk.infra.Configuration
{
    public static class PwnConfigFactory
    {
        
        public static AppConfig Create()
        {
            IConfiguration cfg = new ConfigurationBuilder()
                                        .SetBasePath(Path.GetFullPath("/etc/pwnctl"))
                                        .AddIniFile("config.ini", optional: true, reloadOnChange: true)
                                        .AddSecretsManager(configurator: options => 
                                        {
                                            options.SecretFilter = entry => entry.Name.StartsWith("pwnctl-");
                                            options.KeyGenerator = (entry, s) => s.Replace("pwnctl-", "").Replace("-", ":");
                                        })
                                        .AddEnvironmentVariables(prefix: "PWNCTL_")
                                        .Build();

            var config = cfg.Get<AppConfig>();

            return config;
        }
    }
}
