using pwnctl.app.Tasks.Entities;
using pwnctl.infra.Queues;
using pwnctl.infra.Persistence;
using pwnctl.infra;
using pwnctl.dto.Assets.Commands;
using pwnctl.dto.Mediator;

using MediatR;
using Microsoft.EntityFrameworkCore;
using pwnctl.infra.Logging;
using pwnctl.app.Tasks.Enums;

namespace pwnctl.api.Mediator.Handlers.Assets.Commands
{
    public sealed class ProcessAssetsCommandHandler : IRequestHandler<ProcessAssetsCommand, MediatedResponse<List<TaskRecord>>>
    {
        public async Task<MediatedResponse<List<TaskRecord>>> Handle(ProcessAssetsCommand command, CancellationToken cancellationToken)
        {
            var context = new PwnctlDbContext();

            // swapping the TaskQueueService with a mock that does not queue 
            // anywhere & leaves TakRecords in a PENDING state inorder to 
            // return the tasks to the client for queueing, that way we 
            // eliminate the need for a VpcEndpoint to access the SQS API
            var queueService = new MockTaskQueueService();

            var processor = AssetProcessorFactory.Create(queueService);

            foreach (var asset in command.Assets)
            {
                try
                {
                    await processor.ProcessAsync(asset);
                }
                catch (Exception ex)
                {
                    PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                }            
            }

            var pendingTasks = await context.TaskRecords
                    .Include(r => r.Definition)
                    .Include(r => r.Domain)
                    .Include(r => r.Host)
                    .Include(r => r.NetRange)
                    .Include(r => r.DNSRecord)
                    .Include(r => r.Endpoint)
                    .Include(r => r.Service)
                    .Include(r => r.Keyword)
                    .Include(r => r.CloudService)
                    .Where(r => r.State == TaskState.PENDING)
                    .ToListAsync(cancellationToken);

            pendingTasks.ForEach(task => task.Queued());
            await context.SaveChangesAsync(cancellationToken);

            return MediatedResponse<List<TaskRecord>>.Success(pendingTasks);
        }
    }
}
