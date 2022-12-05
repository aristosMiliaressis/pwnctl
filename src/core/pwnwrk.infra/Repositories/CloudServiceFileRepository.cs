using pwnwrk.domain.Assets.Entities;

namespace pwnwrk.infra.Repositories
{
    public sealed class CloudServiceFileRepository
    {
        private static CloudServiceFileRepository _singleton;
        
        public static CloudServiceFileRepository Singleton
        {
            get
            {
                if (_singleton == null)
                    _singleton = new CloudServiceFileRepository();

                return _singleton;
            }
        }

        public CloudService GetCloudService(string hostname)
        {
            throw new NotImplementedException();
        }
    }
}
