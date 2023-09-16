
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
        private PwnctlDbContext _context = new PwnctlDbContext();
        private static Func<PwnctlDbContext, int, Task<TaskRecord>> FindRecordQuery
                                = EF.CompileAsyncQuery<PwnctlDbContext, int, TaskRecord>(
                        (context, id) => context.TaskRecords
                                    .Include(r => r.Definition)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.Tags)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.NetworkHost)
                                        .ThenInclude(r => r.AARecords)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.NetworkRange)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.NetworkSocket)
                                        .ThenInclude(r => r.NetworkHost)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.NetworkSocket)
                                        .ThenInclude(r => r.DomainName)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.DomainName)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.DomainNameRecord)
                                        .ThenInclude(r => r.DomainName)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.DomainNameRecord)
                                        .ThenInclude(r => r.NetworkHost)
                                    // .Include(r => r.Record)
                                    //     .ThenInclude(r => r.HttpHost)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.HttpEndpoint)
                                        .ThenInclude(s => s.Socket)
                                        .ThenInclude(s => s.DomainName)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.HttpEndpoint)
                                        .ThenInclude(s => s.Socket)
                                        .ThenInclude(s => s.NetworkHost)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.HttpParameter)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.Email)
                                        .ThenInclude(r => r.DomainName)
                                    .Include(r => r.Record)
                                        .ThenInclude(r => r.Scope)
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

        public TaskDbRepository() { }

        public TaskDbRepository(PwnctlDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskRecord>> ListAsync(int pageIdx)
        {
            return await _context.TaskRecords
                                .Include(p => p.Operation)
                                    .ThenInclude(r => r.Policy)
                                    .ThenInclude(r => r.TaskProfiles)
                                    .ThenInclude(r => r.TaskProfile)
                                    .ThenInclude(r => r.TaskDefinitions)
                                .Include(p => p.Definition)
                                .Include(p => p.Record)
                                    .ThenInclude(r => r.NetworkRange)
                                .Include(p => p.Record)
                                    .ThenInclude(r => r.NetworkHost)
                                .Include(p => p.Record)
                                    .ThenInclude(r => r.NetworkSocket)
                                .Include(p => p.Record)
                                    .ThenInclude(r => r.DomainName)
                                .Include(p => p.Record)
                                    .ThenInclude(r => r.DomainNameRecord)
                                // .Include(p => p.Record)
                                //     .ThenInclude(r => r.HttpHost)
                                .Include(p => p.Record)
                                    .ThenInclude(r => r.HttpEndpoint)
                                .Include(p => p.Record)
                                    .ThenInclude(r => r.HttpParameter)
                                .Include(p => p.Record)
                                    .ThenInclude(r => r.Email)
                                .OrderBy(r => r.QueuedAt)
                                .Skip(pageIdx * PwnInfraContext.Config.Api.BatchSize)
                                .Take(PwnInfraContext.Config.Api.BatchSize)
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

        public List<TaskDefinition> ListOutOfScope()
        {
            return _context.TaskDefinitions
                            .Where(d => d.MatchOutOfScope)
                            .ToList();
        }

        public async Task UpdateAsync(TaskRecord task)
        {
            _context.Entry(task).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            _context.Entry(task).DetachReferenceGraph();
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
