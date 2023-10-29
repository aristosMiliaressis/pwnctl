namespace pwnctl.infra.Configuration;

using Microsoft.Extensions.Configuration;
using pwnctl.app.Configuration;

public static class PwnConfigFactory
{
    public static AppConfig Create()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder()
                                    .SetBasePath(EnvironmentVariables.INSTALL_PATH == "" 
                                            ? $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.config/pwnctl" 
                                            : Path.GetFullPath(EnvironmentVariables.INSTALL_PATH))
                                    .AddIniFile("config.ini", optional: true, reloadOnChange: true);

        if (!EnvironmentVariables.USE_LOCAL_INTEGRATIONS)
        {
            builder = builder
                        .AddSystemsManager("/pwnctl")
                        .AddSecretsManager(configurator: options =>
            {
                options.SecretFilter = entry => entry.Name.StartsWith("/aws/secret/pwnctl/");
                options.KeyGenerator = (entry, s) => s.Replace("/aws/secret/pwnctl/", "").Replace("/", ":");
            });
        }

        return builder
                    .AddEnvironmentVariables(prefix: "PWNCTL_")
                    .Build()
                    .Get<AppConfig>();
    }
}