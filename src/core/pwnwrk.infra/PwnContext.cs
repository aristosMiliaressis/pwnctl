using pwnwrk.infra.Configuration;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Repositories;
using pwnwrk.infra.Serialization;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Common.Interfaces;
using pwnwrk.domain.Assets.Interfaces;
using Serilog.Core;

namespace pwnwrk.infra
{
    public static class PwnContext
    {
        static PwnContext()
        {
            Config = PwnConfigFactory.Create();
            Logger = PwnLoggerFactory.Create();
            Serializer = new AppJsonSerializer();
            
            AmbientService<IPublicSuffixRepository>.SetFactory(() => new PublicSuffixRepository());
            AmbientService<IFilterEvaluator>.SetFactory(() => new CSharpFilterEvaluator());
            AmbientService<ISerializer>.SetFactory(() => new AppJsonSerializer());
        }

        public static AppConfig Config { get; private set; }
        public static Logger Logger { get; private set; }
        public static ISerializer Serializer { get; private set; }
    }
}