using pwnctl.dto.Tasks.Queries;
using pwnctl.dto.Tasks.Models;
using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListTaskDefinitionsQueryHandler : IRequestHandler<ListTaskDefinitionsQuery, MediatedResponse<TaskDefinitionListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<TaskDefinitionListViewModel>> Handle(ListTaskDefinitionsQuery command, CancellationToken cancellationToken)
        {
            var tasks = await _context.TaskDefinitions
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);

            return MediatedResponse<TaskDefinitionListViewModel>.Success(new TaskDefinitionListViewModel(tasks));
        }
    }
}