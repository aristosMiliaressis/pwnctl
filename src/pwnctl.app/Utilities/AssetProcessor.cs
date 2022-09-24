using pwnctl.app.Repositories;
using pwnctl.infra.Logging;
using pwnctl.infra.Notifications;
using pwnctl.core.BaseClasses;

namespace pwnctl.app.Utilities
{
    public class AssetProcessor
    {
        private readonly AssetHandlerMap _assetHandlerMap = new();
        private readonly AssetRepository _repository = new();
        private readonly JobAssignmentService _jobService = new();
        private readonly NotificationSender _notificationSender = new();
        private readonly NotificationRuleChecker _notificationRuleChecker = new();

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
                await HandleAssetAsync((BaseAsset)asset);
            }
        }

        private async Task HandleAssetAsync(BaseAsset asset)
        {
            // recursivly process all parsed assets
            // starting from the botton of the ref tree.
            asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(BaseAsset)))
                .Select(rf => (BaseAsset) rf.GetValue(asset))
                .Where(a => a != null)
                .ToList().ForEach(refAsset => HandleAssetAsync(refAsset).Wait());

            // apply class specific processing
            if (_assetHandlerMap[asset.GetType()] != null)
            {
                var handler = _assetHandlerMap[asset.GetType()];

                asset = await handler.HandleAsync(asset);
            }

            asset = await _repository.AddOrUpdateAsync(asset);

            if (asset.InScope)
            {
                var rule = _notificationRuleChecker.Check(asset);
                if (rule != null)
                {
                   _notificationSender.Send(asset, rule);
                }

                await _jobService.AssignAsync(asset);
            }
        }
    }
}
