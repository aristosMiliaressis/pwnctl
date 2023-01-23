using pwnctl.app.Common.Extensions;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.app.Assets.Interfaces;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Assets.Aggregates;
using pwnctl.kernel.Extensions;

namespace pwnctl.infra.Repositories
{
    public sealed class AssetDbRepository : AssetRepository
    {
        private PwnctlDbContext _context;

        public AssetDbRepository(PwnctlDbContext context)
        {
            _context = context;
        }

        public async Task<AssetRecord> FindRecordAsync(Asset asset)
        {
            return await _context.AssetRecords
                            .Include(r => r.Tags)
                            .Include(r => r.NetRange)
                            .Include(r => r.Host)
                            .Include(r => r.Service)
                            .Include(r => r.Domain)
                            .Include(r => r.DNSRecord)
                            .Include(r => r.VirtualHost)
                            .Include(r => r.Endpoint)
                            .Include(r => r.Parameter)
                            .Include(r => r.Email)
                            .FirstOrDefaultAsync(r => r.Id == asset.UID);
        }

        public TaskEntry FindTaskEntry(Asset asset, TaskDefinition def)
        {
            var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset, def);
            return (TaskEntry)_context.FirstNotTrackedFromLambda(lambda);
        }

        public async Task<AssetRecord> MergeCurrentRecordWithDBRecord(AssetRecord record, Asset asset)
        {
            var assetReferences = asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)));

            foreach (var reference in assetReferences)
            {
                var assetRef = (Asset)reference.GetValue(asset);
                if (assetRef == null)
                    continue;

                var existingAsset = _context.FindAsset(assetRef);
                if (existingAsset == null)
                {
                    reference.SetValue(record.Asset, assetRef);
                    continue;
                }

                await _context.Entry(existingAsset).LoadReferencesRecursivelyAsync();

                reference.SetValue(record.Asset, existingAsset);
            };

            return record;
        }

        public async Task SaveAsync(AssetRecord record)
        {
            var existingAsset = _context.FindAsset(record.Asset);
            if (existingAsset == null)
            {
                record.FoundAt = DateTime.UtcNow;

                _context.Entry(record.Asset).State = EntityState.Added;
                _context.Add(record);

                await _context.SaveChangesAsync();

                return;
            }

            _context.Update(record);

            _context.AddRange(record.Tags.Where(t => t.Id == default));

            _context.AddRange(record.Tasks.Where(t => t.Id == default));

            await _context.SaveChangesAsync();
        }

        public async Task<List<AssetRecord>> ListHostsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.Host)
                                .ThenInclude(e => e.AARecords)
                            .Where(r => r.SubjectClass.Class == nameof(Host))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListDomainsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.Domain)
                                .ThenInclude(e => e.ParentDomain)
                            .Where(r => r.SubjectClass.Class == nameof(Domain))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListDNSRecordsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.DNSRecord)
                                .ThenInclude(e => e.Domain)
                            .Include(e => e.DNSRecord)
                                .ThenInclude(e => e.Host)
                            .Where(r => r.SubjectClass.Class == nameof(DNSRecord))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListEndpointsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.Endpoint)
                                .ThenInclude(e => e.Service)
                                    .ThenInclude(s => s.Host)
                            .Include(e => e.Endpoint)
                                .ThenInclude(e => e.Service)
                                    .ThenInclude(s => s.Domain)
                            .Where(r => r.SubjectClass.Class == nameof(Endpoint))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListNetRangesAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.NetRange)
                            .Where(r => r.SubjectClass.Class == nameof(NetRange))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListServicesAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.Service)
                                .ThenInclude(s => s.Host)
                            .Include(e => e.Service)
                                .ThenInclude(s => s.Domain)
                            .Where(r => r.SubjectClass.Class == nameof(Service))
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<AssetRecord>> ListEmailsAsync()
        {
            return await _context.AssetRecords
                            .Include(e => e.Tags)
                            .Include(e => e.Tasks)
                            .Include(e => e.Email)
                                .ThenInclude(e => e.Domain)
                            .Where(r => r.SubjectClass.Class == nameof(Email))
                            .AsNoTracking()
                            .ToListAsync();
        }
    }
}