using pwnctl.dto.Mediator;

using pwnctl.dto.Assets.Commands;
using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets;
using pwnctl.app;
using pwnctl.app.Scope.Entities;
using pwnctl.infra.Persistence;
using pwnctl.infra.Repositories;
using pwnctl.app.Assets.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class ImportAssetsCommandHandler : IRequestHandler<ImportAssetsCommand, MediatedResponse>
    {
        public async Task<MediatedResponse> Handle(ImportAssetsCommand command, CancellationToken cancellationToken)
        {
            PwnctlDbContext context = new();

            var scopeDefinitions = await context.ScopeDefinitions.ToListAsync();

            foreach (var assetText in command.Assets)
            {
                var result = AssetParser.Parse(assetText);
                if (!result.IsOk)
                    continue;

                await RecursiveSave(scopeDefinitions, result.Value);
            }

            return MediatedResponse.Success();
        }

        private async Task RecursiveSave(List<ScopeDefinition> scopeDefinitions, Asset asset) 
        {
            AssetDbRepository assetRepo = new();

            foreach (var childAsset in GetReferencedAssets(asset))
            {
                await RecursiveSave(scopeDefinitions, childAsset);
            }

            var record = new AssetRecord(asset);

            foreach (var def in scopeDefinitions)
            {
                if (def.Matches(record.Asset))
                {
                    record.SetScopeId(def.Id);
                    break;
                }
            }

            await assetRepo.SaveAsync(record);
        }

        private IEnumerable<Asset> GetReferencedAssets(Asset asset)
        {
            var assetProperties = asset.GetType().GetProperties();
            List<Asset> assets = assetProperties
                    .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)))
                    .Select(rf => (Asset?)rf.GetValue(asset))
                    .Where(a => a is not null)
                    .ToList();

            assets.AddRange(assetProperties
                    .Where(p => p.PropertyType.IsGenericType
                            && p.PropertyType.GetGenericArguments()[0].IsAssignableTo(typeof(Asset))
                            && p.GetValue(asset) is not null)
                    .SelectMany(rf => (IEnumerable<Asset>)(rf.GetValue(asset) ?? new List<Asset>())));

            return assets;
        }
    }
}
