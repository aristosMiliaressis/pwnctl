using pwnctl.infra;
using pwnctl.dto.Assets.Commands;
using pwnctl.dto.Mediator;

using MediatR;
using pwnctl.app.Queueing.DTO;
using pwnctl.infra.Repositories;

namespace pwnctl.api.Mediator.Handlers.Assets.Commands
{
    public sealed class ProcessAssetsCommandHandler : IRequestHandler<ProcessAssetsCommand, MediatedResponse<List<PendingTaskDTO>>>
    {
        public async Task<MediatedResponse<List<PendingTaskDTO>>> Handle(ProcessAssetsCommand command, CancellationToken cancellationToken)
        {
            var repo = new TaskDbRepository();

            var processor = AssetProcessorFactory.Create();

            foreach (var asset in command.Assets.Where(a => !string.IsNullOrEmpty(a)))
            {
                await processor.TryProcessAsync(asset);           
            }

            // leaves TaskEntries in a PENDING state inorder to 
            // return the tasks to the client for queueing, that way we 
            // eliminate the need for a VpcEndpoint to access the SQS API
            var pendingTasks = await repo.ListPendingAsync(cancellationToken);
            foreach (var task in pendingTasks)
            {
                task.Queued();
                await repo.UpdateAsync(task);
            }

            return MediatedResponse<List<PendingTaskDTO>>.Success(pendingTasks.Select(t => new PendingTaskDTO(t)).ToList());
        }
    }
}
