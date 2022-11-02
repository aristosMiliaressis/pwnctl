using pwnctl.dto.Targets.Queries;
using pwnctl.dto.Targets.ViewModels;
using pwnctl.dto.Mediator;
using pwnwrk.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.MediatR.Handlers.Targets.Queries
{
    public class ListTargetsQueryHandler : IRequestHandler<ListTargetsQuery, MediatedResponse<TargetListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<TargetListViewModel>> Handle(ListTargetsQuery command, CancellationToken cancellationToken)
        {
            var targets = await _context.Programs
                                        .Include(p => p.Policy)
                                        .Include(p => p.Scope)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);

            return MediatedResponse<TargetListViewModel>.Success(new TargetListViewModel(targets));
        }
    }
}