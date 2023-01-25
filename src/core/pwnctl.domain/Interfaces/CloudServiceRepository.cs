using pwnctl.domain.Entities;

namespace pwnctl.domain.Interfaces;

public interface CloudServiceRepository
{
    static CloudServiceRepository Instance { get; set; }

    bool IsCloudService(HttpEndpoint endpoint);
}