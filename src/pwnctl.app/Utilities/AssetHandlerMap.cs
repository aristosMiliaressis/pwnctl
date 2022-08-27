using pwnctl.core.Interfaces;

namespace pwnctl.app.Utilities
{
    public class AssetHandlerMap : Dictionary<Type, IAssetHandler>
    {
        private Dictionary<Type, IAssetHandler> _map = new();

        public new IAssetHandler this[Type type]
        {
            get => _map.ContainsKey(type) ? _map[type] : null;
            set => _map[type] = value;
        }

        public AssetHandlerMap()
        {
            AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => !t.IsAbstract && t.GetInterfaces().Any(i => i.Name.Contains(nameof(IAssetHandler))))
                    .ToList()
                    .ForEach(handlerType => 
            {
                var assetType = handlerType.BaseType.GetGenericArguments()[0];

                _map[assetType] = Activator.CreateInstance(handlerType) as IAssetHandler;
            });
        }
    }
}