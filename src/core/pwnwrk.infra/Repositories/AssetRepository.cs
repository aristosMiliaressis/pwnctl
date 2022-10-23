using pwnwrk.infra;
using pwnwrk.infra.Extensions;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Persistence.Extensions;
using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Entities.Assets;
using pwnwrk.domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace pwnwrk.infra.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private PwnctlDbContext _context = new();

        public async Task UpdateAsync(BaseAsset asset)
        {
            _context.Update(asset);
            await _context.SaveChangesAsync();
        }

        public async Task AddOrUpdateAsync(BaseAsset asset)
        {
            // replacing asset references from db to prevent ChangeTracker 
            // from trying to add already existing assets and violating 
            // uniqness contraints.
            await asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(BaseAsset)))
                .ToList().ForEachAsync(async reference =>
                {
                    var assetRef = (BaseAsset)reference.GetValue(asset);
                    if (assetRef == null)
                        return;

                    assetRef = await GetAssetWithReferencesAsync(assetRef);
                    reference.SetValue(asset, assetRef);
                });

            if (!CheckIfExists(asset))
            {
                asset.FoundAt = DateTime.Now;

                _context.Add(asset);
            }
            else if (asset.Tags != null)
            {
                var matchingAsset = GetMatchingAsset(asset);
                foreach(var tag in asset.Tags) 
                {
                    var lambda = ExpressionTreeBuilder.BuildTagMatchingLambda(matchingAsset, tag);
                    var existingTag = _context.FirstFromLambda(lambda);
                    if (existingTag == null) 
                    {
                        tag.SetAsset(matchingAsset);
                        _context.Add(tag);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public bool CheckIfExists(BaseAsset asset)
        {
            return GetMatchingAsset(asset) != null;
        }

        public async Task<BaseAsset> GetAssetWithReferencesAsync(BaseAsset asset)
        {
            asset = GetMatchingAsset(asset);
            if (asset == null)
                return null;

            await _context.Entry(asset).LoadReferencesRecursivelyAsync();

            return asset;
        }

        private BaseAsset GetMatchingAsset(BaseAsset asset)
        {
            var lambda = ExpressionTreeBuilder.BuildAssetMatchingLambda(asset);

            return (BaseAsset) _context.FirstFromLambda(lambda);
        }

        public List<Host> ListHosts()
        {
            return _context.Hosts.Include(a => a.Tags).ToList();
        }

        public List<Domain> ListDomains()
        {
            return _context.Domains.Include(a => a.Tags).ToList();
        }

        public List<DNSRecord> ListDNSRecords()
        {
            return _context.DNSRecords.Include(a => a.Tags).ToList();
        }

        public List<Endpoint> ListEndpoints()
        {
            return _context.Endpoints
                            .Include(a => a.Tags)
                            .Include(e => e.Service)
                                .ThenInclude(s => s.Host)
                            .Include(e => e.Service)
                                .ThenInclude(s => s.Domain)
                            .ToList();
        }

        public List<NetRange> ListNetRanges()
        {
            return _context.NetRanges.Include(a => a.Tags).ToList();
        }

        public List<Service> ListServices()
        {
            return _context.Services
                            .Include(a => a.Tags)
                            .Include(e => e.Host)
                            .Include(e => e.Domain)
                            .ToList();
        }

        public List<Email> ListEmails()
        {
            return _context.Emails
                            .Include(a => a.Tags)
                            .Include(e => e.Domain)
                            .ToList();
        }
    }
}