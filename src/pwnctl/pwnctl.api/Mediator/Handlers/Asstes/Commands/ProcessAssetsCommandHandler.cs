using pwnwrk.domain.Tasks.Entities;
using pwnwrk.domain.Tasks.Enums;
using pwnwrk.infra.Queues;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Utilities;
using pwnctl.dto.Assets.Commands;
using pwnctl.dto.Mediator;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.api.Mediator.Handlers.Assets.Commands
{
    public sealed class ProcessAssetsCommandHandler : IRequestHandler<ProcessAssetsCommand, MediatedResponse<List<TaskRecord>>>
    {
        public async Task<MediatedResponse<List<TaskRecord>>> Handle(ProcessAssetsCommand command, CancellationToken cancellationToken)
        {
            var context = new PwnctlDbContext();
            var processor = new AssetProcessor();

            // swapping the JobQueueService with a mock that does not queue 
            // anywhere & leaves TakRecords in a PENDING state inorder to 
            // return the tasks to the client for queueing, that way we 
            // eliminate the need for a VpcEndpoint to access the SQS API
            AssetProcessor.JobQueueService = new MockJobQueueService();

            foreach (var asset in command.Assets)
            {
                await processor.TryProccessAsync(asset);
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
