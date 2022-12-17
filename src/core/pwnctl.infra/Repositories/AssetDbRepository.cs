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
        private PwnctlDbContext _context = new();

        public async Task<AssetRecord> LoadRelatedAssets(Asset asset)
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
                        return;

                    await _context.Entry(assetRef).LoadReferencesRecursivelyAsync((type) => type.IsAssignableTo(typeof(Asset)));

                    reference.SetValue(asset, assetRef);
                });
            
            return new AssetRecord(asset);
        }

        public TaskRecord FindTaskRecord(Asset asset, TaskDefinition def)
        {
            var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset, def);
            return (TaskRecord)_context.FirstNotTrackedFromLambda(lambda);
        }

        public async Task SaveAsync(Asset asset)
        {
            await SaveAsync(new AssetRecord(asset));
        }


        public async Task SaveAsync(AssetRecord record)
        {
            var existingAsset = _context.FindAsset(record.Asset);

            // if asset doesn't exist add it
            if (existingAsset == null)
            {
                record.Asset.FoundAt = DateTime.UtcNow;

                _context.Entry(record.Asset).State = EntityState.Added;
                _context.AddRange(record.Asset.Tags);

                await _context.SaveChangesAsync();

                return;
            }

            // otherwise add new tags/tasks & update the InScope flag
            var newTags = record.Asset.Tags.Where(tag => _context.FindAssetTag(existingAsset, tag) == null);
            newTags.ToList().ForEach(t => t.SetAsset(existingAsset));
            foreach (var tag in newTags)
                _context.Entry(tag).State = EntityState.Added;

            var newTasks = record.Tasks.Where(t => t.Id == default);
            foreach (var task in newTasks) 
                _context.Entry(task).State = EntityState.Added;

            existingAsset.InScope = record.Asset.InScope;
            _context.Update(existingAsset);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Host>> ListHostsAsync()
        {
            return await _context.Hosts
                                .Include(a => a.Tags)
                                .AsNoTracking()
                                .ToListAsync();
        }

        public async Task<List<Domain>> ListDomainsAsync()
        {
            return await _context.Domains
                                .Include(a => a.Tags)
                                .AsNoTracking()
                                .ToListAsync();
        }

        public async Task<List<DNSRecord>> ListDNSRecordsAsync()
        {
            return await _context.DNSRecords
                                .Include(a => a.Tags)
                                .AsNoTracking()
                                .ToListAsync();
        }

        public async Task<List<Endpoint>> ListEndpointsAsync()
        {
            return await _context.Endpoints
                            .Include(a => a.Tags)
                            .Include(e => e.Service)
                                .ThenInclude(s => s.Host)
                            .Include(e => e.Service)
                                .ThenInclude(s => s.Domain)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<NetRange>> ListNetRangesAsync()
        {
            return await _context.NetRanges
                                .Include(a => a.Tags)
                                .AsNoTracking()
                                .ToListAsync();
        }

        public async Task<List<Service>> ListServicesAsync()
        {
            return await _context.Services
                            .Include(a => a.Tags)
                            .Include(e => e.Host)
                            .Include(e => e.Domain)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<Email>> ListEmailsAsync()
        {
            return await _context.Emails
                            .Include(a => a.Tags)
                            .Include(e => e.Domain)
                            .AsNoTracking()
                            .ToListAsync();
        }
    }
}