using pwnctl.dto.Mediator;

using MediatR;
using pwnctl.dto.Assets.Commands;
using pwnctl.app.Assets;
using pwnctl.infra.Persistence;
using pwnctl.app.Assets.Entities;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class ImportAssetsCommandHandler : IRequestHandler<ImportAssetsCommand, MediatedResponse>
    {
        public async Task<MediatedResponse> Handle(ImportAssetsCommand command, CancellationToken cancellationToken)
        {
            PwnctlDbContext context = new();

            foreach (var assetText in command.Assets)
            {
                var asset = AssetParser.Parse(assetText);

                var record = new AssetRecord(asset);

                context.AssetRecords.Add(record);
            }

            await context.SaveChangesAsync();

            return MediatedResponse.Success();
        }
    }
}
