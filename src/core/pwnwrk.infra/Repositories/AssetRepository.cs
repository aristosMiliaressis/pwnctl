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

        public async Task UpdateAsync(Asset asset)
        {
            _context.Update(asset);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateAsync(Asset asset)
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

            if (_context.FindAsset(asset) == null)
            {
                asset.FoundAt = DateTime.UtcNow;

                _context.Add(asset);
            }
            else if (asset.Tags != null)
            {
                var matchingAsset = _context.FindAsset(asset);
                foreach(var tag in asset.Tags) 
                {
                    var existingTag = _context.FindAssetTag(matchingAsset, tag);
                    if (existingTag == null) 
                    {
                        tag.SetAsset(matchingAsset);
                        _context.Add(tag);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Asset> GetAssetWithReferencesAsync(Asset asset)
        {
            asset = _context.FindAsset(asset);
            if (asset == null)
                return null;

            await _context.Entry(asset).LoadReferencesRecursivelyAsync();

            return asset;
        }

        public List<Host> ListHosts()
        {
            return _context.Hosts.Include(a => a.Tags).AsNoTracking().ToList();
        }

        public List<Domain> ListDomains()
        {
            return _context.Domains.Include(a => a.Tags).AsNoTracking().ToList();
        }

        public List<DNSRecord> ListDNSRecords()
        {
            return _context.DNSRecords.Include(a => a.Tags).AsNoTracking().ToList();
        }

        public List<Endpoint> ListEndpoints()
        {
            return _context.Endpoints
                            .Include(a => a.Tags)
                            .Include(e => e.Service)
                                .ThenInclude(s => s.Host)
                            .Include(e => e.Service)
                                .ThenInclude(s => s.Domain)
                            .AsNoTracking()
                            .ToList();
        }

        public List<NetRange> ListNetRanges()
        {
            return _context.NetRanges.Include(a => a.Tags).AsNoTracking().ToList();
        }

        public List<Service> ListServices()
        {
            return _context.Services
                            .Include(a => a.Tags)
                            .Include(e => e.Host)
                            .Include(e => e.Domain)
                            .AsNoTracking()
                            .ToList();
        }

        public List<Email> ListEmails()
        {
            return _context.Emails
                            .Include(a => a.Tags)
                            .Include(e => e.Domain)
                            .AsNoTracking()
                            .ToList();
        }
    }
}