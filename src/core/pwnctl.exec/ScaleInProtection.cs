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
            ExpiresInMinutes = PwnInfraContext.Config.Worker.MaxTaskTimeout / 60
        };

        PwnInfraContext.Logger.Information("Enabling Scale-in Protection");
        var response = await _httpClient.PutAsJsonAsync(Environment.GetEnvironmentVariable("ECS_AGENT_URI")+"/task-protection/v1/state", request);
        if(!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            PwnInfraContext.Logger.Warning($"Failed to enable Scale-in Protection Response status {response.StatusCode}");
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

        PwnInfraContext.Logger.Information("Disabling Scale-in Protection");
        var response = await _httpClient.PutAsJsonAsync(Environment.GetEnvironmentVariable("ECS_AGENT_URI")+"/task-protection/v1/state", request);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            PwnInfraContext.Logger.Warning($"Failed to disable Scale-in Protection Response status {response.StatusCode}");
            PwnInfraContext.Logger.Warning(responseBody);
        }
    }
}