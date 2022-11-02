using pwnwrk.domain.Assets.Entities;

namespace pwnwrk.infra.Repositories
{
    public sealed class CloudServiceRepository
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

        public CloudService GetCloudService(string hostname)
        {
            throw new NotImplementedException();
        }
    }
}
