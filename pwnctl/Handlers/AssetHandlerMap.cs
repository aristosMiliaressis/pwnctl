using pwnctl;
using pwnctl.Persistence;
using pwnctl.Entities;
using pwnctl.Handlers;
using pwnctl.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pwnctl.Handlers
{
    public class AssetHandlerMap : Dictionary<Type, IAssetHandler>
    {
        private Dictionary<Type, IAssetHandler> _map = new();

        public new IAssetHandler this[Type type]
        {
            get => _map[type];
            set => _map[type] = value;
        }

        public AssetHandlerMap()
        {
            AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsClass && t.GetInterfaces().Any(i => i.Name.Contains(nameof(IAssetHandler))))
                    .ToList()
                    .ForEach(handlerType => 
            {
                var assetType = handlerType.GetInterfaces()
                                    .First(i => i.IsGenericType && i.Name.Contains(nameof(IAssetHandler)))
                                    .GetGenericArguments()[0];

                _map[assetType] = Activator.CreateInstance(handlerType) as IAssetHandler;
            });
        }
    }
}