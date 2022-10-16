using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using System.Text.Json;

namespace pwnwrk.infra.Configuration
{
    public static class PwnConfigFactory
    {
        public static AppConfig Create()
        {
            try
            {
                Directory.CreateDirectory(AppConfig.InstallPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create installation directory {AppConfig.InstallPath}", ex);
            }

            IConfiguration cfg = new ConfigurationBuilder()
                                        .SetBasePath(Path.GetFullPath(AppConfig.InstallPath))
                                        .AddIniFile("config.ini", optional: true, reloadOnChange: true)
                                        .AddEnvironmentVariables(prefix: "PWNCTL_")
                                        .Build();

            return cfg.Get<AppConfig>();
        }
    }
}
