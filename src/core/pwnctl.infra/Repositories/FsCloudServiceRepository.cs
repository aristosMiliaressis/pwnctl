using pwnctl.app;
using pwnctl.app.Scope.Entities;
using pwnctl.infra.Configuration;
using pwnctl.domain.Entities;
using pwnctl.domain.Interfaces;

namespace pwnctl.infra.Repositories
{
    public sealed class FsCloudServiceRepository : CloudServiceRepository
    {
        private static string _cloudServiceDataFile = $"{EnvironmentVariables.InstallPath}/wordlists/cloud-services.json";
        private List<CloudService> _services;

        public bool IsCloudService(HttpEndpoint endpoint)
        {
            if (_services == null)
                _services = PwnInfraContext.Serializer.Deserialize<List<CloudService>>(File.ReadAllText(_cloudServiceDataFile));

            return _services.SelectMany(s => s.Scope).Any(s => s.Matches(endpoint));
        }

        public class CloudService
        {
            public string Service { get; set; }
            public string Provider { get; set; }
            public List<ScopeDefinition> Scope { get; set; }
        }
    }
}
