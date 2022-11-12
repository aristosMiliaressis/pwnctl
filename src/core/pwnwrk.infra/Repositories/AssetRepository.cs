using pwnwrk.infra.Extensions;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Persistence.Extensions;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.Entities;
using Microsoft.EntityFrameworkCore;

namespace pwnwrk.infra.Repositories
{
    public sealed class AssetRepository : IAssetRepository
    {
        private PwnctlDbContext _context = new();

        public async Task SaveAsync(Asset asset)
        {
            // replacing asset references from db to prevent ChangeTracker 
            // from trying to add already existing assets and violating 
            // uniqness contraints.
            await asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)))
                .ToList().ForEachAsync(async reference =>
                {
                    var assetRef = (Asset)reference.GetValue(asset);
                    if (assetRef == null)
                        return;

                    assetRef = await GetAssetWithReferencesAsync(assetRef);
                    reference.SetValue(asset, assetRef);
                });

            // if asset doesn't exist add it
            if (_context.FindAsset(asset) == null)
            {
                asset.FoundAt = DateTime.UtcNow;

                _context.Add(asset);

                await _context.SaveChangesAsync();

                return;
            }

            // otherwise add any new tags & tasks

            if (asset.Tags != null)
            {
                var matchingAsset = _context.FindAsset(asset);
                foreach (var tag in asset.Tags)
                {
                    var existingTag = _context.FindAssetTag(matchingAsset, tag);
                    if (existingTag == null)
                    {
                        tag.SetAsset(matchingAsset);
                        _context.Add(tag);
                    }
                }
            }

            if (asset.Tasks != null)
            {
                var matchingAsset = _context.FindAsset(asset);
                foreach (var task in asset.Tasks)
                {
                    var existingTask = _context.FindAssetTaskRecord(matchingAsset, task.Definition);
                    if (existingTask == null)
                    {
                        _context.Entry(task.Definition).State = EntityState.Unchanged;
                        _context.Add(task);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<Asset> GetAssetWithReferencesAsync(Asset asset)
        {
            asset = _context.FindAsset(asset);
            if (asset == null)
                return null;

            await _context.Entry(asset).LoadReferencesRecursivelyAsync();

            return asset;
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