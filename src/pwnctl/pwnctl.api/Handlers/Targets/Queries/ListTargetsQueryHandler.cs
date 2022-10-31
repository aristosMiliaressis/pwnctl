namespace pwnctl.api.Handlers.Targets.Queries;

using pwnctl.dto.Targets.Queries;
using pwnctl.dto.Targets.ViewModels;
using pwnwrk.infra.MediatR;
using pwnwrk.infra.Persistence;

using MediatR;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class ListTargetsQueryHandler : IRequestHandler<ListTargetsQuery, MediatorResult<TargetListViewModel>>
{
    private readonly PwnctlDbContext _context = new PwnctlDbContext();

    public async Task<MediatorResult<TargetListViewModel>> Handle(ListTargetsQuery command, CancellationToken cancellationToken)
    {
        Console.WriteLine("ListTargetsQueryHandler");
        var targets = await _context.Programs.AsNoTracking().ToListAsync(cancellationToken);
        Console.WriteLine("MediatorResult");

        return MediatorResult<TargetListViewModel>.Success(new TargetListViewModel(targets));
    }
}