using pwnwrk.infra.Configuration;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Repositories;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Assets.Interfaces;
using System.Reflection;
using Serilog.Core;

namespace pwnwrk.infra
{
    public static class PwnContext
    {
        static PwnContext()
        {
            Config = PwnConfigFactory.Create();
            Logger = PwnLoggerFactory.Create();

            AmbientService<IPublicSuffixRepository>.SetFactory(() => new PublicSuffixRepository());
            AmbientService<IFilterEvaluator>.SetFactory(() => new CSharpFilterEvaluator());
        }

        public static AppConfig Config { get; private set; }
        public static Logger Logger { get; private set; }
    }
}