namespace pwnctl.api.Handlers.Targets.Queries;

using pwnctl.dto.Targets.Queries;
using pwnctl.dto.Targets.ViewModels;
using pwnwrk.infra.MediatR;
using pwnwrk.infra.Persistence;

using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class ListTargetsQueryHandler : IRequestHandler<ListTargetsQuery, MediatorResponse<TargetListViewModel>>
{
    private readonly PwnctlDbContext _context;

    public ListTargetsQueryHandler(PwnctlDbContext context)
    {
        _context = context;
    }

    public async Task<MediatorResponse<TargetListViewModel>> Handle(ListTargetsQuery command, CancellationToken cancellationToken)
    {
        var targets = await _context.Programs.ToListAsync(cancellationToken);

        return MediatorResponse<TargetListViewModel>.Success(new TargetListViewModel(targets));
    }
}