using pwnwrk.domain.Entities;
using pwnwrk.domain.BaseClasses;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Persistence.Extensions;
using pwnwrk.infra.Queues;
using pwnwrk.domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace pwnwrk.infra.Utilities
{
    public class JobAssignmentService
    {
        private static readonly IJobQueueService _jobQueueService = JobQueueFactory.Create();
        private readonly PwnctlDbContext _context = new();
        private readonly ScopeChecker _scopeChecker;
        private readonly List<TaskDefinition> _taskDefinitions;

        public JobAssignmentService()
        {
            _taskDefinitions = _context.TaskDefinitions.ToList();
            var scopeDefinitions = _context.ListScopeDefinitions();

            _scopeChecker = new ScopeChecker(scopeDefinitions);
        }

        public async System.Threading.Tasks.Task AssignAsync(BaseAsset asset)
        {
            var program = _scopeChecker.GetApplicableProgram(asset);
            if (program == null)
                 return;

            var whitelist = program.Policy.Whitelist?.Split(",") ?? new string[0];
            var blacklist = program.Policy.Blacklist?.Split(",") ?? new string[0];

            foreach (var definition in _taskDefinitions.Where(d => d.Subject == asset.GetType().Name))
            {
                if (blacklist.Contains(definition.ShortName))
                {
                    continue;
                }
                else if (whitelist.Contains(definition.ShortName))
                {
                    await EnqueueJobAsync(definition, asset);
                    continue;
                }
                else if (definition.IsActive && !program.Policy.AllowActive)
                {
                    continue;
                }
                else if (definition.Aggressiveness <= program.Policy.MaxAggressiveness)
                {
                    await EnqueueJobAsync(definition, asset);
                }
            }
        }

        private async System.Threading.Tasks.Task EnqueueJobAsync(TaskDefinition definition, BaseAsset asset)
        {
            if (!string.IsNullOrEmpty(definition.Filter) && !CSharpScriptHelper.Evaluate(definition.Filter, asset))
            {
                return;
            }

            // only queue one task per TaskDefinition/Asset pair
            var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset, definition);
            var task = (pwnwrk.domain.Entities.Task) _context.FirstFromLambda(lambda);
            if (task != null)
                return;

            task = new pwnwrk.domain.Entities.Task(definition, asset);
            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            await _jobQueueService.EnqueueAsync(task);
        }
    }
}
