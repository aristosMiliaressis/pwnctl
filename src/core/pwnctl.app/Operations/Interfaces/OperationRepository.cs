using pwnctl.app.Operations.Entities;

namespace pwnctl.app.Operations.Interfaces;

public interface OperationRepository
{
    Task<Operation?> FindAsync(int id);

    Task SaveAsync(Operation op);
}
