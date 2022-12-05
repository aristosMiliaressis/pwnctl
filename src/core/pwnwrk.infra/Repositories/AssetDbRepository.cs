using pwnwrk.infra.Extensions;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Persistence.Extensions;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.domain.Assets.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace pwnwrk.infra.Repositories
{
    public sealed class AssetDbRepository : AssetRepository
    {
        private PwnctlDbContext _context = new();

        public async Task<Asset> LoadRelatedAssets(Asset asset)
        {
            await asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(Asset)))
                .ToList().ForEachAsync(async reference =>
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
            
            return asset;
        }

        public async Task SaveAsync(Asset asset)
        {
            var existingAsset = _context.FindAsset(asset);

            // if asset doesn't exist add it
            if (existingAsset == null)
            {
                asset.FoundAt = DateTime.UtcNow;

                _context.Entry(asset).State = EntityState.Added;
                _context.AddRange(asset.Tags);

                await _context.SaveChangesAsync();

                return;
            }

            // otherwise add new tags & update the InScope flag
            var newTags = asset.Tags.Where(tag => _context.FindAssetTag(asset, tag) == null);
            _context.AddRange(newTags);

            existingAsset.InScope = asset.InScope;
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