using pwnctl.dto.Targets.Queries;
using pwnctl.dto.Targets.ViewModels;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListTasksQueryHandler : IRequestHandler<ListTasksQuery, MediatedResponse<TasksViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<TasksViewModel>> Handle(ListTasksQuery command, CancellationToken cancellationToken)
        {
            var tasks = await _context.TaskEntries
                                        .Include(p => p.Definition)
                                        .Include(p => p.Record)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);

            return MediatedResponse<TasksViewModel>.Success(new TasksViewModel(tasks));
        }
    }
}