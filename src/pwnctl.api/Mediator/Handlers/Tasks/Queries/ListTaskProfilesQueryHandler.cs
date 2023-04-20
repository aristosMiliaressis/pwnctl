using pwnctl.dto.Tasks.Queries;
using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListTaskProfilesQueryHandler : IRequestHandler<ListTaskProfilesQuery, MediatedResponse<TaskProfileListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<TaskProfileListViewModel>> Handle(ListTaskProfilesQuery command, CancellationToken cancellationToken)
        {
            var tasks = await _context.TaskProfiles
                                        .Include(p => p.TaskDefinitions)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);

            return MediatedResponse<TaskProfileListViewModel>.Success(new TaskProfileListViewModel(tasks));
        }
    }
}