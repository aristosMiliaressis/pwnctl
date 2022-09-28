using pwnctl.app.Repositories;
using pwnctl.core.Entities;
using pwnctl.core.BaseClasses;
using pwnctl.infra.Persistence;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.infra;
using pwnctl.core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace pwnctl.app.Utilities
{
    public class JobAssignmentService
    {
        private static readonly IJobQueueService _jobQueueService = JobQueueFactory.Create();
        private readonly PwnctlDbContext _context = new();
        private readonly List<TaskDefinition> _taskDefinitions;

        public JobAssignmentService()
        {
            _taskDefinitions = _context.TaskDefinitions.ToList();
        }

        public async System.Threading.Tasks.Task AssignAsync(BaseAsset asset)
        {
            var program = ScopeChecker.Singleton.GetApplicableProgram(asset);
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
            // check filter
            if (!string.IsNullOrEmpty(definition.Filter) && !CSharpScriptHelper.Evaluate(definition.Filter, asset))
            {
                return;
            }

            // check if task has already been queued
            var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset, definition);
            var task = (core.Entities.Task) _context.FirstFromLambda(lambda);
            if (task != null)
                return;

            task = new core.Entities.Task(definition, asset);
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            await _jobQueueService.EnqueueAsync(task);
        }
    }
}
