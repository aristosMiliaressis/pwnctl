using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Common;
using pwnctl.app.Tasks.Entities;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Persistence.IdGenerators;

using Microsoft.EntityFrameworkCore;

namespace pwnctl.infra.Repositories
{
    public sealed class AssetDbRepository : AssetRepository, IDisposable
    {
        private PwnctlDbContext _context;

        public AssetDbRepository()
        {
            _context = new PwnctlDbContext();
        }

        public AssetDbRepository(PwnctlDbContext context)
        {
            _context = context;
        }

        public async Task<AssetRecord> FindRecordAsync(Asset asset)
        {
            return await _context.AssetRecords
                            .Include(r => r.Tags)
                            .Include(r => r.FoundByTask)
                            .Include(r => r.NetworkRange)
                            .Include(r => r.NetworkHost)
                            .Include(r => r.NetworkSocket)
                            .Include(r => r.DomainName)
                            .Include(r => r.DomainNameRecord)
                            .Include(r => r.HttpHost)
                            .Include(r => r.HttpEndpoint)
                            .Include(r => r.HttpParameter)
                            .Include(r => r.Email)
                            .FirstOrDefaultAsync(r => r.Id == HashIdValueGenerator.Generate(asset));
        }

        public Asset FindMatching(Asset asset)
        {
            var lambda = ExpressionTreeBuilder.BuildAssetMatchingLambda(asset);

            return (Asset) _context.FirstFromLambda(lambda);
        }

        public TaskEntry FindTaskEntry(Asset asset, TaskDefinition def)
        {
            var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset, def);
            return (TaskEntry)_context.FirstNotTrackedFromLambda(lambda);
        }

        public async Task<AssetRecord> UpdateRecordReferences(AssetRecord record, Asset asset)
        {
            var assetReferences = asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset))
                         && p.GetValue(asset) != null);

            foreach (var reference in assetReferences)
            {
                var existing = FindMatching((Asset)reference.GetValue(asset));
                if (existing == null)
                    continue;

                await _context.Entry(existing).LoadReferencesRecursivelyAsync();

                reference.SetValue(record.Asset, existing);
            };

            var existingAsset = FindMatching(record.Asset);
            if (existingAsset == null)
                return record;

            var assetCollections = asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsGenericType
                         && p.PropertyType.GetGenericArguments()[0].IsAssignableTo(typeof(Asset))
                         && p.GetValue(asset) != null);

            foreach (var collection in assetCollections)
            {
                var innerAssets = (IEnumerable<Asset>)collection.GetValue(asset);
                if (innerAssets == null || !innerAssets.Any())
                {
                    await _context.Entry(existingAsset).LoadReferencesRecursivelyAsync();
                    collection.SetValue(record.Asset, collection.GetValue(existingAsset));
                }
            }

            return record;
        }

        public async Task SaveAsync(AssetRecord record)
        {
            record.Program = null;
            record.Tasks.ForEach(t => 
            {
                if (t.Definition != null)
                    _context.Entry(t.Definition).State = EntityState.Unchanged;
            });

            var existingAsset = FindMatching(record.Asset);
            if (existingAsset == null)
            {
                record.FoundAt = DateTime.UtcNow;

                _context.Entry(record.Asset).State = EntityState.Added;
                
                _context.Add(record);
            }
            else
            {
                _context.Entry(record.Asset).DetachReferechGraph();
                _context.Entry(record).State = EntityState.Modified;

                _context.AddRange(record.Tags.Where(t => t.Id == default));

                _context.AddRange(record.Tasks.Where(t => t.Id == default));
            }

            await _context.SaveChangesAsync();

            _context.Entry(record).DetachReferechGraph();
            record.Tasks.ForEach(t =>
            {
                if (t.Definition != null)
                    _context.Entry(t.Definition).State = EntityState.Detached;
            });
        }

        public async Task<List<AssetRecord>> ListHostsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.NetworkHost)
                                .ThenInclude(e => e.AARecords)
                                .ThenInclude(e => e.DomainName)
                            .Where(r => r.SubjectClass.Class == nameof(NetworkHost))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListDomainsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.DomainName)
                                .ThenInclude(e => e.ParentDomain)
                            .Where(r => r.SubjectClass.Class == nameof(DomainName))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListDNSRecordsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.DomainNameRecord)
                                .ThenInclude(e => e.DomainName)
                            .Include(e => e.DomainNameRecord)
                                .ThenInclude(e => e.NetworkHost)
                            .Where(r => r.SubjectClass.Class == nameof(DomainNameRecord))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListEndpointsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.HttpEndpoint)
                                .ThenInclude(e => e.Socket)
                                    .ThenInclude(s => s.NetworkHost)
                            .Include(e => e.HttpEndpoint)
                                .ThenInclude(e => e.Socket)
                                    .ThenInclude(s => s.DomainName)
                            .Where(r => r.SubjectClass.Class == nameof(HttpEndpoint))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListNetRangesAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.NetworkRange)
                            .Where(r => r.SubjectClass.Class == nameof(NetworkRange))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListParametersAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.HttpParameter)
                                .ThenInclude(e => e.Endpoint)
                                .ThenInclude(e => e.Socket)
                                .ThenInclude(s => s.NetworkHost)
                            .Include(e => e.HttpParameter)
                                .ThenInclude(e => e.Endpoint)
                                .ThenInclude(e => e.Socket)
                                .ThenInclude(s => s.DomainName)
                            .Where(r => r.SubjectClass.Class == nameof(HttpParameter))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListServicesAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.NetworkSocket)
                                .ThenInclude(s => s.NetworkHost)
                            .Include(e => e.NetworkSocket)
                                .ThenInclude(s => s.DomainName)
                            .Where(r => r.SubjectClass.Class == nameof(NetworkSocket))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListEmailsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.Email)
                                .ThenInclude(e => e.DomainName)
                            .Where(r => r.SubjectClass.Class == nameof(Email))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}