using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Common;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Persistence.IdGenerators;

using Microsoft.EntityFrameworkCore;
using pwnctl.app.Notifications.Entities;
using pwnctl.domain.ValueObjects;
using System.Data;

namespace pwnctl.infra.Repositories
{
    public sealed class AssetDbRepository : AssetRepository
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

        private static Func<PwnctlDbContext, Guid, Task<AssetRecord>> FindRecordQuery
                            = EF.CompileAsyncQuery<PwnctlDbContext, Guid, AssetRecord>(
                    (context, id) => context.AssetRecords
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
                                            .FirstOrDefault(r => r.Id == id));


        public async Task<AssetRecord> FindRecordAsync(Asset asset)
        {
            return await FindRecordQuery(_context, UUIDv5ValueGenerator.GenerateByString(asset.ToString()));
        }

        public async Task<List<AssetRecord>> ListInScopeAsync(int scopeId, AssetClass[] assetClasses, int pageIdx, int pageSize = 4096, CancellationToken token = default)
        {
            var aggregate = await _context.ScopeAggregates
                                    .Include(a => a.Definitions)
                                        .ThenInclude(d => d.Definition)
                                    .FirstOrDefaultAsync(a => a.Id == scopeId);

            var scopeDefinitionIds = aggregate.Definitions.Select(d => d.DefinitionId).ToList();

            return await _context.AssetRecords
                                .Include(r => r.Tasks)
                                    .ThenInclude(t => t.Definition)
                                .Include(r => r.NetworkRange)
                                .Include(r => r.NetworkHost)
                                .Include(r => r.NetworkSocket)
                                .Include(r => r.DomainName)
                                .Include(r => r.DomainNameRecord)
                                .Include(r => r.HttpHost)
                                .Include(r => r.HttpEndpoint)
                                .Include(r => r.HttpParameter)
                                .Include(r => r.Email)
                                .Where(a => assetClasses.Contains(a.Subject) && a.ScopeId.HasValue && scopeDefinitionIds.Contains(a.ScopeId.Value))
                                .OrderBy(a => a.FoundAt)
                                .Skip(pageIdx * pageSize)
                                .Take(pageSize)
                                .ToListAsync(token);
        }

        public Asset FindMatching(Asset asset)
        {
            var lambda = ExpressionTreeBuilder.BuildAssetMatchingLambda(asset);

            return _context.FirstFromLambda<Asset>(lambda);
        }

        public async Task<Notification> FindNotificationAsync(Asset asset, NotificationRule rule)
        {
            var lambda = ExpressionTreeBuilder.BuildNotificationMatchingLambda(asset.Id, rule.Id);

            return await _context.FirstNotTrackedFromLambdaAsync<Notification>(lambda);
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

                await _context.Entry(existing).LoadReferenceGraphAsync();

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
                    await _context.Entry(existingAsset).LoadReferenceGraphAsync();
                    collection.SetValue(record.Asset, collection.GetValue(existingAsset));
                }
            }

            return record;
        }

        public async Task SaveAsync(AssetRecord record)
        {
            // TaskDefinitions are not tracked as a slight performance improvement
            // so we have to null them to prevent the change tracker from attempting to add them
            record.Notifications.ForEach(t => t.Task = null);
            record.Tasks.ForEach(t => t.Definition = null);

            // this prevents race conditions when checking if the record already exists
            using (var trx = _context.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                var existingRecord = await FindRecordAsync(record.Asset);
                if (existingRecord == null)
                {
                    // explicitly track asset to prevent change tracker from 
                    // navigating related assets and attempting to add them.
                    _context.Entry(record.Asset).State = EntityState.Added;

                    _context.Add(record);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (record.InScope)
                        existingRecord.SetScopeId(record.ScopeId.Value);

                    // explicitly track record to prevent change tracker from 
                    // navigating related assets and attempting to add them.
                    _context.Entry(existingRecord).State = EntityState.Modified;

                    _context.AddRange(record.Tags.Where(t => t.Id == default).Select(t => 
                    {
                        t.RecordId = existingRecord.Id;
                        return t;
                    }));

                    _context.AddRange(record.Tasks.Where(t => t.Id == default).Select(t => 
                    {
                        t.Record = existingRecord;
                        t.RecordId = existingRecord.Id;
                        return t;
                    }));

                    _context.AddRange(record.Notifications.Where(t => t.Id == default).Select(t => 
                    {
                        t.Record = existingRecord;
                        t.RecordId = existingRecord.Id;
                        return t;
                    }));

                    await _context.SaveChangesAsync();
                }
                
                trx.Commit();
            }

            _context.Entry(record).DetachReferenceGraph();
        }

        public async Task<List<AssetRecord>> ListHostsAsync(int pageIdx, int pageSize = 4096)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.NetworkHost)
                                .ThenInclude(e => e.AARecords)
                                .ThenInclude(e => e.DomainName)
                            .Where(r => r.Subject == AssetClass.Create(nameof(NetworkHost)))
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx*pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListDomainsAsync(int pageIdx, int pageSize = 4096)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.DomainName)
                                .ThenInclude(e => e.ParentDomain)
                            .Where(r => r.Subject == AssetClass.Create(nameof(DomainName)))
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListDNSRecordsAsync(int pageIdx, int pageSize = 4096)
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
                            .Where(r => r.Subject == AssetClass.Create(nameof(DomainNameRecord)))
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListEndpointsAsync(int pageIdx, int pageSize = 4096)
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
                            .Where(r => r.Subject == AssetClass.Create(nameof(HttpEndpoint)))
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListNetRangesAsync(int pageIdx, int pageSize = 4096)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.NetworkRange)
                            .Where(r => r.Subject == AssetClass.Create(nameof(NetworkRange)))
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListParametersAsync(int pageIdx, int pageSize = 4096)
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
                            .Where(r => r.Subject == AssetClass.Create(nameof(HttpParameter)))
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListServicesAsync(int pageIdx, int pageSize = 4096)
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
                            .Where(r => r.Subject == AssetClass.Create(nameof(NetworkSocket)))
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListEmailsAsync(int pageIdx, int pageSize = 4096)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.Email)
                                .ThenInclude(e => e.DomainName)
                            .Where(r => r.Subject == AssetClass.Create(nameof(Email)))
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * pageSize)
                            .Take(pageSize)
                            .AsNoTracking()
                            .ToListAsync();
        }
    }
}
