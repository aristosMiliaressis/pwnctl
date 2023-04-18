using pwnctl.dto.Operations.Queries;
using pwnctl.dto.Operations.ViewModels;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Operations.Queries
{
    public sealed class ListOperationsQueryHandler : IRequestHandler<ListOperationsQuery, MediatedResponse<OperationListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<OperationListViewModel>> Handle(ListOperationsQuery command, CancellationToken cancellationToken)
        {
            var targets = await _context.Operations
                                        .Include(p => p.Policy)
                                        .Include(p => p.Scope)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);

            return MediatedResponse<OperationListViewModel>.Success(new OperationListViewModel(targets));
        }
    }
}