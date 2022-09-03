using pwnctl.app.Utilities;
using pwnctl.infra;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Repositories;
using pwnctl.infra.Logging;
using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;
using pwnctl.core.Entities;
using pwnctl.core.Interfaces;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq.Expressions;

namespace pwnctl.app.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private PwnctlDbContext _context = new();

        public async Task<BaseAsset> AddOrUpdateAsync(BaseAsset asset)
        {
            _context.ChangeTracker.Clear();

            // replace asset references from db
            // this prevents some database errors.
            asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(BaseAsset)))
                .ToList().ForEach(reference =>
                {
                    var assetRef = (BaseAsset)reference.GetValue(asset);
                    if (assetRef == null)
                        return;

                    assetRef = GetAssetWithReferences(assetRef);
                    if (assetRef != null)
                        reference.SetValue(asset, assetRef);
                });

            if (!CheckIfExists(asset))
            {
                asset.FoundAt = DateTime.Now;
                asset.InScope = ScopeChecker.Singleton.IsInScope(asset);

                _context.Add(asset);
            }
            else if (asset.Tags != null)
            {
                var matchingAsset = GetMatchingAsset(asset);
                foreach(var tag in asset.Tags) 
                {
                    var lambda = ExpressionTreeBuilder.BuildTagMatchingLambda(matchingAsset, tag);
                    var existingTag = _context.FirstFromLambda(lambda);
                    if (existingTag == null) 
                    {
                        tag.SetAsset(matchingAsset);
                        _context.Add(tag);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return GetAssetWithReferences(asset);
        }

        public bool CheckIfExists(BaseAsset asset)
        {
            return GetMatchingAsset(asset) != null;
        }

        public BaseAsset GetMatchingAsset(BaseAsset asset)
        {
            var lambda = ExpressionTreeBuilder.BuildAssetMatchingLambda(asset);

            return (BaseAsset)_context.FirstFromLambda(lambda);
        }

        public BaseAsset GetAssetWithReferences(BaseAsset asset)
        {
            asset = GetMatchingAsset(asset);
            if (asset == null)
                return null;
            _context.Entry(asset).LoadReferencesRecursivelyAsync().Wait();

            return asset;
        }
    }
}