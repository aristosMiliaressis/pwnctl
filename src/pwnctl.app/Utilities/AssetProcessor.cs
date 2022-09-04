using pwnctl.app.Repositories;
using pwnctl.infra.Logging;
using pwnctl.core.BaseClasses;
using Newtonsoft.Json;

namespace pwnctl.app.Utilities
{
    public class AssetProcessor
    {
        private readonly AssetHandlerMap _assetHandlerMap = new();
        private readonly AssetRepository _repository = new();
        private readonly JobAssignmentService _jobService = new();

        public async Task<bool> TryProccessAsync(string assetText)
        {
            try
            {
                await ProcessAsync(assetText);
                return true;
            } 
            catch (Exception ex)
            {
                Logger.Instance.Info(ex.ToRecursiveExInfo());
                return false;
            }
        }

        public async Task ProcessAsync(string assetText)
        {
            bool parsed = AssetParser.TryParse(assetText, out Type[] assetTypes, out BaseAsset[] assets);
            if (!parsed)
            {
                return;
            }

            foreach (var asset in assets)
            {
                await HandleAsset((BaseAsset)asset);
            }
        }

        private async Task HandleAsset(BaseAsset asset)
        {
            // recursivly process all parsed assets
            // starting from the botton of the ref tree.
            asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(BaseAsset)))
                .Select(rf => (BaseAsset) rf.GetValue(asset))
                .Where(a => a != null)
                .ToList().ForEach(async refAsset => await HandleAsset(refAsset));

            // apply class specific processing
            if (_assetHandlerMap[asset.GetType()] != null)
            {
                var handler = _assetHandlerMap[asset.GetType()];

                asset = await handler.HandleAsync(asset);
            }

            asset = await _repository.AddOrUpdateAsync(asset);

            if (asset.InScope)
            {
                _jobService.Assign(asset);
            }
        }
    }
}
