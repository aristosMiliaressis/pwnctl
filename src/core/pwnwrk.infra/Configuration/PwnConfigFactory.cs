using Microsoft.Extensions.Configuration;

using pwnwrk.infra.Aws;

namespace pwnwrk.infra.Configuration
{
    public static class PwnConfigFactory
    {
        
        public static AppConfig Create()
        {
            var installPath = Environment.GetEnvironmentVariable("PWNCTL_InstallPath");

            IConfiguration cfg = new ConfigurationBuilder()
                                        .SetBasePath(Path.GetFullPath(installPath))
                                        .AddIniFile("config.ini", optional: true, reloadOnChange: true)
                                        .AddSecretsManager(configurator: options => 
                                        {
                                            options.SecretFilter = entry => entry.Name.StartsWith("pwnctl-");
                                            options.KeyGenerator = (entry, s) => s.Replace("pwnctl-", "").Replace("-", ":");
                                        })
                                        .AddEnvironmentVariables(prefix: "PWNCTL_")
                                        .Build();

            return cfg.Get<AppConfig>();
        }
    }
}
