using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Assets.Interfaces;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Common;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra.Persistence.IdGenerators;

using Microsoft.EntityFrameworkCore;
using pwnctl.app.Notifications.Entities;
using pwnctl.domain.ValueObjects;
using System.Data;
using Npgsql;
using pwnctl.app;
using pwnctl.kernel.Extensions;

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
                                            // needed for in-scope decision
                                            .Include(r => r.NetworkHost)
                                                .ThenInclude(h => h.AARecords)
                                                .ThenInclude(r => r.DomainName)
                                            .Include(r => r.NetworkSocket)
                                                .ThenInclude(s => s.NetworkHost)
                                                .ThenInclude(h => h.AARecords)
                                                .ThenInclude(r => r.DomainName)
                                            .FirstOrDefault(r => r.Id == id));


        public async Task<AssetRecord> FindRecordAsync(Asset asset)
        {
            return await FindRecordQuery(_context, UUIDv5ValueGenerator.GenerateByString(asset.ToString()));
        }

        public async Task<List<AssetRecord>> ListInScopeAsync(int scopeId, AssetClass[] assetClasses, int pageIdx, CancellationToken token = default)
        {
            var aggregate = await _context.ScopeAggregates
                                    .Include(a => a.Definitions)
                                        .ThenInclude(d => d.Definition)
                                    .FirstOrDefaultAsync(a => a.Id == scopeId);

            var scopeDefinitionIds = aggregate.Definitions.Select(d => d.DefinitionId).ToList();

            return await _context.AssetRecords
                                .Include(r => r.Tasks)
                                    .ThenInclude(t => t.Definition)
                                .Where(a => assetClasses.Contains(a.Subject) && a.ScopeId.HasValue && scopeDefinitionIds.Contains(a.ScopeId.Value))
                                .OrderBy(a => a.FoundAt)
                                .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                                .Take(PwnInfraContext.Config.Api.BatchSize)
                                .ToListAsync(token);
        }

        public Asset FindMatching(Asset asset)
        {
            var lambda = ExpressionTreeBuilder.BuildAssetMatchingLambda(asset);

            return _context.FirstFromLambda<Asset>(lambda);
        }

        public async Task<AssetRecord> UpdateRecordReferences(AssetRecord record, Asset asset)
        {
            var assetReferences = asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset))
                         && p.GetValue(asset) is not null);

            foreach (var reference in assetReferences)
            {
                var existing = FindMatching((Asset)reference.GetValue(asset));
                if (existing is null)
                    continue;

                await _context.Entry(existing).LoadReferenceGraphAsync();

                reference.SetValue(record.Asset, existing);
            };

            var existingAsset = FindMatching(record.Asset);
            if (existingAsset is null)
                return record;

            var assetCollections = asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsGenericType
                         && p.PropertyType.GetGenericArguments()[0].IsAssignableTo(typeof(Asset))
                         && p.GetValue(asset) is not null);

            foreach (var collection in assetCollections)
            {
                var innerAssets = (IEnumerable<Asset>)collection.GetValue(asset);
                if (innerAssets is null || !innerAssets.Any())
                {
                    await _context.Entry(existingAsset).LoadReferenceGraphAsync();
                    collection.SetValue(record.Asset, collection.GetValue(existingAsset));
                }
            }

            return record;
        }

        public async Task<IEnumerable<TaskRecord>> SaveAsync(AssetRecord record)
        {
            try
            {
                var existingRecord = await FindRecordAsync(record.Asset);
                if (existingRecord is null)
                {
                    // explicitly track asset to prevent change tracker from
                    // navigating related assets and attempting to add them.
                    _context.Entry(record.Asset).State = EntityState.Added;

                    _context.Add(record);

                    await _context.SaveChangesAsync();

                    return record.Tasks;
                }

                if (record.InScope)
                {
                    _context.AssetRecords
                            .FromSqlRaw($"""UPDATE asset_records SET "InScope" = true, "ScopeId" = %s WHERE "Id" = %s;""",
                                    record.ScopeId, record.Id);
                }

                _context.AddRange(record.Tags.Where(t => !existingRecord.Tags.Select(t => t.Name).Contains(t.Name)).Select(t =>
                {
                    t.Record = existingRecord;
                    t.RecordId = existingRecord.Id;
                    return t;
                }));

                var newTasks = record.Tasks.Where(t => t.Id == default).ToList();
                _context.AddRange(newTasks.Select(t =>
                {
                    t.Record = null;
                    t.RecordId = existingRecord.Id;
                    return t;
                }));

                _context.AddRange(record.Notifications.Where(t => t.Id == default).Select(t =>
                {
                    t.Record = null;
                    t.RecordId = existingRecord.Id;
                    return t;
                }));

                await _context.SaveChangesAsync();

                return newTasks;
            }
            finally
            {
                // detaching record references to avoid an ever growing
                // change tracking graph and reduce momory consumption.
                _context.Entry(record.Asset).DetachReferenceGraph();
                _context.Entry(record).State = EntityState.Detached;
            }
        }

        public async Task<IEnumerable<AssetRecord>> ListNetworkHostsAsync(int pageIdx, CancellationToken token = default)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Where(r => r.NetworkHostId != null)
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx*PwnInfraContext.Config.Api.BatchSize)
                            .Take(PwnInfraContext.Config.Api.BatchSize)
                            .AsNoTracking()
                            .ToListAsync(token);
        }

        public async Task<IEnumerable<AssetRecord>> ListDomainNamesAsync(int pageIdx, CancellationToken token = default)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Where(r => r.DomainNameId != null)
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                            .Take(PwnInfraContext.Config.Api.BatchSize)
                            .AsNoTracking()
                            .ToListAsync(token);
        }

        public async Task<IEnumerable<AssetRecord>> ListDomainNameRecordsAsync(int pageIdx, CancellationToken token = default)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Where(r => r.DomainNameRecordId != null)
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                            .Take(PwnInfraContext.Config.Api.BatchSize)
                            .AsNoTracking()
                            .ToListAsync(token);
        }

        public async Task<IEnumerable<AssetRecord>> ListHttpEndpointsAsync(int pageIdx, CancellationToken token = default)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Where(r => r.HttpEndpointId != null)
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                            .Take(PwnInfraContext.Config.Api.BatchSize)
                            .AsNoTracking()
                            .ToListAsync(token);
        }

        public async Task<IEnumerable<AssetRecord>> ListNetworkRangesAsync(int pageIdx, CancellationToken token = default)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Where(r => r.NetworkRangeId != null)
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                            .Take(PwnInfraContext.Config.Api.BatchSize)
                            .AsNoTracking()
                            .ToListAsync(token);
        }

        public async Task<IEnumerable<AssetRecord>> ListHttpParametersAsync(int pageIdx, CancellationToken token = default)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Where(r => r.HttpParameterId != null)
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                            .Take(PwnInfraContext.Config.Api.BatchSize)
                            .AsNoTracking()
                            .ToListAsync(token);
        }

        public async Task<IEnumerable<AssetRecord>> ListNetworkSocketsAsync(int pageIdx, CancellationToken token = default)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Where(r => r.NetworkSocketId != null)
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                            .Take(PwnInfraContext.Config.Api.BatchSize)
                            .AsNoTracking()
                            .ToListAsync(token);
        }

        public async Task<IEnumerable<AssetRecord>> ListEmailsAsync(int pageIdx, CancellationToken token = default)
        {
            return await _context.AssetRecords
                            .Include(e => e.FoundByTask)
                                .ThenInclude(e => e.Definition)
                            .Include(e => e.Tags)
                            .Where(r => r.EmailId != null)
                            .OrderBy(r => r.FoundAt)
                            .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                            .Take(PwnInfraContext.Config.Api.BatchSize)
                            .AsNoTracking()
                            .ToListAsync(token);
        }
    }

    // public async Task SaveSafeAsync(AssetRecord record)
    // {
    //     try
    //     {
    //         // this prevents concurrency conflicts when checking if the record already exists
    //         using (var trx = _context.Database.BeginTransaction(IsolationLevel.Serializable)) // TODO: try Snapshot? why?
    //         {
    //             var existingRecord = await FindRecordAsync(record.Asset);
    //             if (existingRecord is null)
    //             {
    //                 // explicitly track asset to prevent change tracker from
    //                 // navigating related assets and attempting to add them.
    //                 _context.Entry(record.Asset).State = EntityState.Added;

    //                 _context.Add(record);

    //                 await _context.SaveChangesAsync();

    //                 trx.Commit();
    //                 return;
    //             }

    //             if (record.InScope)
    //             {
    //                 _context.AssetRecords
    //                         .FromSqlRaw("UPDATE asset_records SET \"InScope\" = true, \"ScopeId\" = %s, \"ConcurrencyToken\" = %s WHERE \"ScopeId\" != %s AND \"ConcurrencyToken\" = "
    //                                 + "(SELECT \"ConcurrencyToken\" FROM asset_records WHERE \"ConcurrencyToken\" = %s FOR UPDATE SKIP LOCKED);",
    //                                 record.ScopeId, Guid.NewGuid(), record.ScopeId, existingRecord.ConcurrencyToken);
    //             }

    //             _context.AddRange(record.Tags.Where(t => !existingRecord.Tags.Select(t => t.Name).Contains(t.Name)).Select(t =>
    //             {
    //                 t.Record = existingRecord;
    //                 t.RecordId = existingRecord.Id;
    //                 return t;
    //             }));

    //             _context.AddRange(record.Tasks.Where(t => t.Id == default).Select(t =>
    //             {
    //                 t.Record = null;
    //                 t.RecordId = existingRecord.Id;
    //                 return t;
    //             }));

    //             _context.AddRange(record.Notifications.Where(t => t.Id == default).Select(t =>
    //             {
    //                 t.Record = null;
    //                 t.RecordId = existingRecord.Id;
    //                 return t;
    //             }));

    //             await _context.SaveChangesAsync();

    //             trx.Commit();
    //         }
    //     }
    //     catch (Exception ex) when (ex is DbUpdateException || ex is PostgresException || ex is DBConcurrencyException || ex is InvalidOperationException)
    //     {
    //         PwnInfraContext.Logger.Warning(ex.ToRecursiveExInfo());
            
    //         // optimistic concurrency yolo!
    //     }
    //     finally
    //     {
    //         // detaching record references to avoid an ever growing
    //         // change tracking graph and reduce momory consumption.
    //         _context.Entry(record.Asset).DetachReferenceGraph();
    //         _context.Entry(record).State = EntityState.Detached;
    //     }
    // }
}
