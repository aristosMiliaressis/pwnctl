using Microsoft.EntityFrameworkCore;
using pwnctl.dto.Mediator;
using pwnwrk.infra.Persistence;
using pwnwrk.infra.Utilities;
using pwnctl.dto.Process.Commands;

using MediatR;

namespace pwnctl.api.MediatR.Handlers.Process.Commands
{
    public class ProcessAssetsCommandHandler : IRequestHandler<ProcessAssetsCommand, MediatedResponse>
    {
        public async Task<MediatedResponse> Handle(ProcessAssetsCommand command, CancellationToken cancellationToken)
        {
            var processor = new AssetProcessor();

            foreach (var asset in command.Assets)
            {
                await processor.TryProccessAsync(asset);
            }

            return MediatedResponse.Success();
        }
    }
}
