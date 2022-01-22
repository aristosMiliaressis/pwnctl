using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pwnctl.Entities;
using pwnctl.DataEF;
using pwnctl.Services;
using pwnctl.Parsers;

namespace pwnctl.Handlers
{
    public class DomainHandler : IAssetHandler<Domain>
    {
        private readonly JobQueueService _queueService = new();
        private readonly PwntainerDbContext _context = new();

        public DomainHandler() {}

        public async Task HandleAsync(IAsset asset)
        {
            var domain = asset as Domain;

            var existingDomain = await _context.Domains.FirstOrDefaultAsync(e => e.Name == domain.Name);
            if (existingDomain != null)
            {
                return;
            }

            // TODO: Program.ScopeDefinition.Pattern

            if (!domain.IsRegistrationDomain)
            {
                var registrationDomain = await _context.Domains.FirstOrDefaultAsync(d => d.Name == DomainNameParser.GetRegistrationDomain(domain.Name));
                if (registrationDomain == null)
                {
                    registrationDomain = new Domain(DomainNameParser.GetRegistrationDomain(domain.Name));
                    _context.Domains.Add(registrationDomain);
                }
                else
                {
                    domain.RegistrationDomain = registrationDomain;
                }
            }

            _context.Domains.Add(domain);

            if (domain.InScope)
            {
                _queueService.Enqueue($"dig +short {domain.Name} | pwnctl");
            }

            await _context.SaveChangesAsync();
        }
    }
}