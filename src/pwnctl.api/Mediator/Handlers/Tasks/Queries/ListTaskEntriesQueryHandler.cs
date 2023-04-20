using pwnctl.dto.Tasks.Queries;
using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListTaskEntriesQueryHandler : IRequestHandler<ListTaskEntriesQuery, MediatedResponse<TaskEntryListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<TaskEntryListViewModel>> Handle(ListTaskEntriesQuery command, CancellationToken cancellationToken)
        {
            var tasks = await _context.TaskEntries
                                        .Include(p => p.Definition)
                                        .Include(p => p.Record)
                                            .ThenInclude(r => r.NetworkRange)
                                        .Include(p => p.Record)
                                            .ThenInclude(r => r.NetworkHost)
                                        .Include(p => p.Record)
                                            .ThenInclude(r => r.NetworkSocket)
                                        .Include(p => p.Record)
                                            .ThenInclude(r => r.DomainName)
                                        .Include(p => p.Record)
                                            .ThenInclude(r => r.DomainNameRecord)
                                        .Include(p => p.Record)
                                            .ThenInclude(r => r.HttpHost)
                                        .Include(p => p.Record)
                                            .ThenInclude(r => r.HttpEndpoint)
                                        .Include(p => p.Record)
                                            .ThenInclude(r => r.HttpParameter)
                                        .Include(p => p.Record)
                                            .ThenInclude(r => r.Email)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);

            return MediatedResponse<TaskEntryListViewModel>.Success(new TaskEntryListViewModel(tasks));
        }
    }
}