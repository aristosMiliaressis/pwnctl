using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Commands;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.DependencyInjection;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class SeedDbCommandHandler : IRequestHandler<SeedDbCommand, MediatedResponse>
    {
        public async Task<MediatedResponse> Handle(SeedDbCommand command, CancellationToken cancellationToken)
        {
            PwnInfraContextInitializer.Setup();

            await DatabaseInitializer.InitializeAsync();

            return MediatedResponse.Success();
        }
    }
}