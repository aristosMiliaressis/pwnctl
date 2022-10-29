namespace pwnctl.api.Handlers.Process.Commands;

using Microsoft.EntityFrameworkCore;
using pwnwrk.infra.MediatR;
using pwnwrk.infra.Persistence;
//using pwnwrk.cli.Utilities;
using pwnctl.dto.Process.Commands;

using MediatR;

public class ProcessAssetsCommandHandler : IRequestHandler<ProcessAssetsCommand, MediatorResult>
{
    private readonly PwnctlDbContext _context;

    public ProcessAssetsCommandHandler(PwnctlDbContext context)
    {
        _context = context;
    }

    public Task<MediatorResult> Handle(ProcessAssetsCommand command, CancellationToken cancellationToken)
    {
        //var processor = new AssetProcessor();

        throw new NotImplementedException();
    }
}