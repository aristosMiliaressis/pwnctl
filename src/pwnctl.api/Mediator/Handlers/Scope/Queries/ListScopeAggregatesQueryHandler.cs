using pwnctl.dto.Scope.Queries;
using pwnctl.dto.Scope.Models;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Scope.Queries
{
    public sealed class ListScopeAggregatesQueryHandler : IRequestHandler<ListScopeAggregatesQuery, MediatedResponse<ScopeListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<ScopeListViewModel>> Handle(ListScopeAggregatesQuery command, CancellationToken cancellationToken)
        {
            var aggregates = await _context.ScopeAggregates
                                        .Include(p => p.Definitions)
                                            .ThenInclude(r => r.Definition)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);

            return MediatedResponse<ScopeListViewModel>.Success(new ScopeListViewModel(aggregates));
        }
    }
}