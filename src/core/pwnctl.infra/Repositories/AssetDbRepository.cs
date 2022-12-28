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
using pwnctl.app.Tagging.Entities;

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
            if (asset.Id == default)
            {
                asset = _context.FindAsset(asset);
                if (asset == null)
                    return null;
            }

            return await _context.AssetRecords
                            .Include(r => r.Tags)
                            .FirstOrDefaultAsync(r => r.Id == asset.Id);
        }

        public Tag FindTag(Asset asset, Tag tag)
        {
            var lambda = ExpressionTreeBuilder.BuildTagMatchingLambda(asset, tag);
            return (Tag)_context.FirstNotTrackedFromLambda(lambda);
        }

        public TaskEntry FindTaskEntry(Asset asset, TaskDefinition def)
        {
            var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset, def);
            return (TaskEntry)_context.FirstNotTrackedFromLambda(lambda);
        }

        public async Task<AssetRecord> MergeCurrentRecordWithDBRecord(AssetRecord record, Asset asset)
        {
            await asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)))
                .ForEachAsync(async reference =>
                {
                    var assetRef = (Asset)reference.GetValue(asset);
                    if (assetRef == null)
                        return;

                    assetRef = _context.FindAsset(assetRef);
                    if (assetRef == null)
                    {
                        reference.SetValue(record.Asset, assetRef);
                        return;
                    }

                    await _context.Entry(assetRef).LoadReferencesRecursivelyAsync((type) => type.IsAssignableTo(typeof(Asset)));

                    reference.SetValue(record.Asset, assetRef);
                });

            return record;
        }

        public async Task SaveAsync(AssetRecord record)
        {
            var existingAsset = _context.FindAsset(record.Asset);
            if (existingAsset == null)
            {
                record.FoundAt = DateTime.UtcNow;

                _context.Entry(record.Asset).State = EntityState.Added; // TODO: Track "Domain" refs from Tests.cs
                _context.Add(record);

                await _context.SaveChangesAsync();

                return;
            }

            _context.AddRange(record.Tags.Where(t => t.Id == default));

            _context.AddRange(record.Tasks.Where(t => t.Id == default));

            _context.Update(record);

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