
using pwnctl.app.Tasks.Entities;
using pwnctl.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Tasks.Interfaces;
using pwnctl.infra.Persistence.Extensions;
using pwnctl.app;
using pwnctl.app.Common;
using pwnctl.app.Assets.Entities;

namespace pwnctl.infra.Repositories
{
    public sealed class TaskDbRepository : TaskRepository
    {
        private PwnctlDbContext _context;
        
        private static Func<PwnctlDbContext, int, Task<TaskRecord>> FindRecordQuery
                                = EF.CompileAsyncQuery<PwnctlDbContext, int, TaskRecord>(
                        (context, id) => context.TaskRecords
                                    .Include(r => r.Definition)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.Tags)
                                    .Include(r => r.Operation)
                                        .ThenInclude(r => r.Policy)
                                        .ThenInclude(r => r.TaskProfiles)
                                        .ThenInclude(r => r.TaskProfile)
                                        .ThenInclude(r => r.TaskDefinitions)
                                    .Include(r => r.Operation)
                                        .ThenInclude(o => o.Scope)
                                        .ThenInclude(o => o.Definitions)
                                        .ThenInclude(o => o.Definition)
                                    .FirstOrDefault(r => r.Id == id));

        public TaskDbRepository() 
        { 
            _context = new();
        }

        public TaskDbRepository(PwnctlDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskRecord>> ListAsync(int pageIdx)
        {
            return await _context.TaskRecords
                                .Include(p => p.Operation)
                                    .ThenInclude(r => r.Policy)
                                    .ThenInclude(r => r.TaskProfiles)
                                    .ThenInclude(r => r.TaskProfile)
                                    .ThenInclude(r => r.TaskDefinitions)
                                .Include(p => p.Definition)
                                .Include(p => p.Record)
                                .OrderBy(r => r.QueuedAt)
                                .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                                .Take(PwnInfraContext.Config.Api.BatchSize)
                                .AsNoTracking()
                                .ToListAsync();
        }

        public async Task<IEnumerable<TaskRecord>> ListPhaseTasksAsync(int opId, int phase)
        {
            return await _context.TaskRecords
                                .Include(p => p.Definition)
                                    .ThenInclude(p => p.Profile)
                                .Include(p => p.Record)
                                .Where(t => t.OperationId == opId && t.Definition.Profile.Phase == phase)
                                .AsNoTracking()
                                .ToListAsync();
        }

        public async Task<TaskRecord> FindAsync(int taskId)
        {
            return await FindRecordQuery(_context, taskId);
        }

        public TaskRecord Find(AssetRecord asset, TaskDefinition definition)
        {
            var lambda = ExpressionTreeBuilder.BuildTaskMatchingLambda(asset.Id, definition.Id);

            return _context.FirstFromLambda<TaskRecord>(lambda);
        }

        public async Task<bool> TryUpdateAsync(TaskRecord task)
        {
            try 
            {
                _context.Entry(task).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _context.Entry(task).DetachReferenceGraph();
                return true;
            }
            catch (Exception ex) 
            {
                PwnInfraContext.Logger.Exception(ex);
                return false;
            }
        }

        public async Task AddAsync(TaskRecord task)
        {
            if (task.RecordId == default)
                throw new ArgumentException($"AssetRecord must be saved first!");

            _context.Entry(task).State = EntityState.Added;

            await _context.SaveChangesAsync();

            _context.Entry(task).DetachReferenceGraph();
        }
    }
}
