using pwnctl.app.Utilities;
using pwnctl.infra;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.core.BaseClasses;
using pwnctl.core.Entities.Assets;
using pwnctl.core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.app.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private PwnctlDbContext _context = new();

        public BaseAsset AddOrUpdate(BaseAsset asset)
        {
            // creating new instance to prevent concurrency issues.
            _context = new();

            // replacing asset references from db to prevent ChangeTracker 
            // from trying to add already existing assets and violating 
            // uniqness contraints.
            asset.GetType()
                .GetProperties()
                .Where(p => p.PropertyType.IsAssignableTo(typeof(BaseAsset)))
                .ToList().ForEach(reference =>
                {
                    var assetRef = (BaseAsset)reference.GetValue(asset);
                    if (assetRef == null)
                        return;

                    assetRef = GetAssetWithReferences(assetRef);
                    if (assetRef != null)
                        reference.SetValue(asset, assetRef);
                });

            if (!CheckIfExists(asset))
            {
                asset.FoundAt = DateTime.Now;
                asset.InScope = ScopeChecker.Singleton.IsInScope(asset);

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

            _context.SaveChanges();

            return GetAssetWithReferences(asset);
        }

        public bool CheckIfExists(BaseAsset asset)
        {
            return GetMatchingAsset(asset) != null;
        }

        public BaseAsset GetMatchingAsset(BaseAsset asset)
        {
            var lambda = ExpressionTreeBuilder.BuildAssetMatchingLambda(asset);

            return (BaseAsset)_context.FirstFromLambda(lambda);
        }

        public BaseAsset GetAssetWithReferences(BaseAsset asset)
        {
            asset = GetMatchingAsset(asset);
            if (asset == null)
                return null;
            _context.Entry(asset).LoadReferencesRecursivelyAsync().Wait();

            return asset;
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