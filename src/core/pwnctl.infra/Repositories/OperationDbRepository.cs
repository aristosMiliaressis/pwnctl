namespace pwnctl.infra.Repositories;

using Microsoft.EntityFrameworkCore;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Interfaces;
using pwnctl.infra.Persistence;

public class OperationDbRepository : OperationRepository
{
    private static PwnctlDbContext _context = new();

    public async Task<Operation> FindAsync(int id)
    {
        return await _context.Operations
                            .Include(o => o.Policy)
                                .ThenInclude(p => p.TaskProfile)
                                .ThenInclude(p => p.TaskDefinitions)
                            .FirstOrDefaultAsync(o => o.Id == id);
    }
}