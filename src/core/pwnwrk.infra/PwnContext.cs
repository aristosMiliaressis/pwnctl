using pwnwrk.infra.Configuration;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Repositories;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.BaseClasses;
using Serilog.Core;

namespace pwnwrk.infra
{
    public static class PwnContext
    {
        static PwnContext()
        {
            Config = PwnConfigFactory.Create();
            Logger = PwnLoggerFactory.Create();

            Register(() => new PublicSuffixRepository(), 
                     () => new CSharpFilterEvaluator());
        }

        public static void Register(params AmbientService<IAmbientService>.AmbientServiceFactory[] factories)
        {
            foreach (var factory in factories)
            {
                var service = factory();

                service
                    .GetType()
                    .GetMethod(nameof(AmbientService<IAmbientService>.SetFactory))
                    .Invoke(service, new object[] { factory });
            }
        }

        public static AppConfig Config { get; private set; }
        public static Logger Logger { get; private set; }
    }
}