﻿using pwnctl.app.Exceptions;
using pwnctl.app.Repositories;
using pwnctl.infra.Extensions;
using pwnctl.infra.Logging;
using pwnctl.infra.Notifications;
using pwnctl.core.BaseClasses;

namespace pwnctl.app.Utilities
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
                Logger.Instance.Info(ex.ToRecursiveExInfo());
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
                .ToList().ForEachAsync(async refAsset => 
                {
                    await HandleAssetAsync(refAsset);
                });

            await _repository.AddOrUpdateAsync(asset);

            // load asset references to be used in scope checking process
            asset = await _repository.GetAssetWithReferencesAsync(asset);

            asset.InScope = ScopeChecker.Singleton.IsInScope(asset);
            if (asset.InScope)
            {
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
}
