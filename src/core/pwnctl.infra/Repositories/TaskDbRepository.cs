
using pwnctl.app.Tasks.Entities;
using pwnctl.infra.Persistence;
using Microsoft.EntityFrameworkCore;
using pwnctl.app.Tasks.Enums;

namespace pwnctl.infra.Repositories
{
    public sealed class TaskDbRepository : IDisposable
    {
        private PwnctlDbContext _context = new PwnctlDbContext();
        public IQueryable<TaskEntry> JoinedQueryable => _context.TaskEntries
                            .Include(r => r.Record)
                                .ThenInclude(r => r.Program)
                            .Include(r => r.Definition)
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
                            .Include(r => r.Record)
                                .ThenInclude(r => r.HttpHost)
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
                                .ThenInclude(r => r.DomainName);

        public async Task<TaskEntry> GetEntryAsync(int taskId)
        {
            return await JoinedQueryable
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.Id == taskId);
        }

        public async Task<List<TaskEntry>> ListPendingAsync(CancellationToken token = default)
        {
            return await JoinedQueryable
                        .Where(r => r.State == TaskState.PENDING)
                        .ToListAsync(token);
        }

        public async Task UpdateAsync(TaskEntry task)
        {
            if (_context.Entry(task).State == EntityState.Detached)
                _context.Entry(task).State = EntityState.Modified;
            
            await _context.SaveChangesAsync();
            _context.Entry(task).State = EntityState.Detached;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}