using pwnwrk.infra.Configuration;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.domain.Interfaces;
using pwnwrk.domain.ValueObjects;

namespace pwnwrk.infra.Repositories
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
