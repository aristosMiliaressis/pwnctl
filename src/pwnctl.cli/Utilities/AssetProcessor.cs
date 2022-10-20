using pwnctl.cli.Exceptions;
using pwnctl.cli.Repositories;
using pwnwrk.infra;
using pwnwrk.infra.Extensions;
using pwnwrk.infra.Logging;
using pwnwrk.infra.Notifications;
using pwnwrk.domain.BaseClasses;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace pwnctl.cli.Utilities
{
    public class AssetProcessor
    {
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
                PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                return false;
            }
        }

        public async Task ProcessAsync(string assetText)
        {
            bool parsed = AssetParser.TryParse(assetText, out Type[] assetTypes, out BaseAsset[] assets);
            if (!parsed)
            {
                throw new UnparsableAssetException(assetText);
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
            await asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(BaseAsset)))
                .Select(rf => (BaseAsset) rf.GetValue(asset))
                .Where(a => a != null)
                .ToList()
                .ForEachAsync(async refAsset => 
                {
                    await HandleAssetAsync(refAsset);
                });

            await _repository.AddOrUpdateAsync(asset);

            // load asset references to be used in scope checking process
            asset = await _repository.GetAssetWithReferencesAsync(asset);

            asset.InScope = ScopeChecker.Singleton.IsInScope(asset);
            if (!asset.InScope)
            {
                return;
            }

            var rule = _notificationRuleChecker.Check(asset);
            if (rule != null)
            {
                _notificationSender.Send(asset, rule);
            }

            await _jobService.AssignAsync(asset);

            await _repository.UpdateAsync(asset);
        }
    }
}
