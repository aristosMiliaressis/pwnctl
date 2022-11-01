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
            Console.WriteLine("ListTargetsQueryHandler");
            var targets = await _context.Programs.AsNoTracking().ToListAsync(cancellationToken);
            Console.WriteLine("MediatedResponse");

            return MediatedResponse<TargetListViewModel>.Success(new TargetListViewModel(targets));
        }
    }
}