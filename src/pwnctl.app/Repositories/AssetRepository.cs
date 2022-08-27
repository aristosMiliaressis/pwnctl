using pwnctl.app.Utilities;
using pwnctl.infra;
using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using pwnctl.infra.Logging;
using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;
using pwnctl.core.Entities.Assets;
using pwnctl.core.Interfaces;
using Newtonsoft.Json;
using System.Reflection;
using System.Linq.Expressions;

namespace pwnctl.app.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private PwnctlDbContext _context = new();

        public async Task<BaseAsset> AddAsync(BaseAsset asset)
        {
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

                    assetRef = GetAsset(assetRef);
                    if (assetRef != null)
                        reference.SetValue(asset, assetRef);
                });

            asset.FoundAt = DateTime.Now;
            asset.InScope = ScopeChecker.Singleton.IsInScope(asset);
            _context.Add(asset);

            await _context.SaveChangesAsync();

            return asset;
        }

        public bool CheckIfExists(BaseAsset asset)
        {
            return GetAsset(asset) != null;
        }

        public BaseAsset GetAsset(BaseAsset asset)
        {
            var lambda = ExpressionTreeBuilder.BuildAssetMatchingLambda(asset);

            return (BaseAsset) _context.FirstFromLambda(lambda);
        }
    }
}