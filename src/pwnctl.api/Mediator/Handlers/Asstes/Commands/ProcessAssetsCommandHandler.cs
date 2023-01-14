using pwnctl.app;
using pwnctl.infra.Persistence;
using pwnctl.infra;
using pwnctl.dto.Assets.Commands;
using pwnctl.dto.Mediator;

using MediatR;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Tasks.Enums;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.app.Queueing.DTO;

namespace pwnctl.api.Mediator.Handlers.Assets.Commands
{
    public sealed class ProcessAssetsCommandHandler : IRequestHandler<ProcessAssetsCommand, MediatedResponse<List<QueueTaskDTO>>>
    {
        public async Task<MediatedResponse<List<QueueTaskDTO>>> Handle(ProcessAssetsCommand command, CancellationToken cancellationToken)
        {
            var context = new PwnctlDbContext();

            var processor = AssetProcessorFactory.Create();

            foreach (var asset in command.Assets.Where(a => !string.IsNullOrEmpty(a)))
            {
                await processor.TryProcessAsync(asset);           
            }

            // leaves TakRecords in a PENDING state inorder to 
            // return the tasks to the client for queueing, that way we 
            // eliminate the need for a VpcEndpoint to access the SQS API
            var pendingTasks = await context.JoinedTaskRecordQueryable()
                    .Where(r => r.State == TaskState.PENDING)
                    .ToListAsync(cancellationToken);

            pendingTasks.ForEach(task => task.Queued());
            await context.SaveChangesAsync(cancellationToken);

            return MediatedResponse<List<QueueTaskDTO>>.Success(pendingTasks.Select(t => new QueueTaskDTO(t)).ToList());
        }
    }
}
