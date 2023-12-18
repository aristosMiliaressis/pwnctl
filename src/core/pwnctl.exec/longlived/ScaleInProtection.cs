using pwnctl.app;

namespace pwnctl.exec;

public static class ScaleInProtection
{
    private static HttpClient _httpClient = new();

    public static async Task EnableAsync()
    {
        if (Environment.GetEnvironmentVariable("ECS_AGENT_URI") == null)
            return;

        var request = new 
        {
            ProtectionEnabled = true,
            ExpiresInMinutes = 2880
        };

        PwnInfraContext.Logger.Information("Enabling scale-in protection.");
        var response = await _httpClient.PutAsJsonAsync(Environment.GetEnvironmentVariable("ECS_AGENT_URI")+"/task-protection/v1/state", request);
        if(!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            PwnInfraContext.Logger.Warning($"Failed to enable scale-in protection response status {response.StatusCode}.");
            PwnInfraContext.Logger.Warning(responseBody);
        }
    }

    public static async Task DisableAsync()
    {
        if (Environment.GetEnvironmentVariable("ECS_AGENT_URI") == null)
            return;

        var request = new
        {
            ProtectionEnabled = false
        };

        PwnInfraContext.Logger.Information("Disabling scale-in protection.");
        var response = await _httpClient.PutAsJsonAsync(Environment.GetEnvironmentVariable("ECS_AGENT_URI")+"/task-protection/v1/state", request);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            PwnInfraContext.Logger.Warning($"Failed to disable scale-in protection response status {response.StatusCode}.");
            PwnInfraContext.Logger.Warning(responseBody);
        }
    }
}