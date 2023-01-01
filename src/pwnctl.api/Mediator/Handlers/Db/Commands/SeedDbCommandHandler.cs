using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Commands;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.app;
using pwnctl.infra.Configuration;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class SeedDbCommandHandler : IRequestHandler<SeedDbCommand, MediatedResponse>
    {
        public async Task<MediatedResponse> Handle(SeedDbCommand command, CancellationToken cancellationToken)
        {
            PwnInfraContext.Config = PwnConfigFactory.Create();

            await DatabaseInitializer.InitializeAsync();

            return MediatedResponse.Success();
        }
    }
}