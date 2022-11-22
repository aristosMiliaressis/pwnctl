using pwnctl.dto.Mediator;
using pwnwrk.infra.Utilities;
using pwnctl.dto.Assets.Commands;

using MediatR;

namespace pwnctl.api.Mediator.Handlers.Assets.Commands
{
    public sealed class ProcessAssetsCommandHandler : IRequestHandler<ProcessAssetsCommand, MediatedResponse>
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