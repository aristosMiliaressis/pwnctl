using pwnctl.app.Repositories;
using pwnctl.core.Entities;
using pwnctl.core.BaseClasses;
using pwnctl.infra.Persistence;
using pwnctl.infra.Logging;
using pwnctl.infra;
using pwnctl.core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace pwnctl.app.Utilities
{
    public class JobAssignmentService
    {
        private static readonly IJobQueueService _jobQueueService = JobQueueFactory.Create();
        private readonly PwnctlDbContext _context = new();
        private readonly AssetRepository _repository = new();
        private readonly List<TaskDefinition> _taskDefinitions;
        private readonly List<ScopeDefinition> _scopeDefinitions;

        public JobAssignmentService()
        {
            _taskDefinitions = _context.TaskDefinitions.ToList();
            _scopeDefinitions = _context.ScopeDefinitions
                                .Include(s => s.Program)
                                .ThenInclude(p => p.Policy).ToList();
        }

        public void Assign(BaseAsset asset)
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
                    queueJob(definition, asset);
                    continue;
                }
                else if (definition.IsActive && !program.Policy.AllowActive)
                {
                    continue;
                }
                else if (definition.Aggressiveness <= program.Policy.MaxAggressiveness)
                {
                    queueJob(definition, asset);
                }
            }

        }

        private void queueJob(TaskDefinition definition, BaseAsset asset)
        {
            if (!string.IsNullOrEmpty(definition.Filter) && !CSharpScriptHelper.Evaluate(definition.Filter, asset))
            {
                return;
            }

            var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset, definition);
            var task = (core.Entities.Task)_context.FirstFromLambda(lambda);
            if (task != null)
                return;

            task = new core.Entities.Task(definition, asset);

            _jobQueueService.Enqueue(task);

            _context.Tasks.Add(task);
            _context.SaveChanges();
        }
    }
}
