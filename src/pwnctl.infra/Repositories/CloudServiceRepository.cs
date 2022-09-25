using pwnctl.infra.Configuration;
using pwnctl.core.Entities.Assets;
using pwnctl.core.Interfaces;
using pwnctl.core.ValueObjects;

namespace pwnctl.infra.Repositories
{
    public class CloudServiceRepository
    {
        private static CloudServiceRepository _singleton;
        
        public static CloudServiceRepository Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new CloudServiceRepository();

                return _singleton;
            }
        }

        public CloudService GetCloudService(string domainname)
        {
            throw new NotImplementedException();
        }
    }
}
