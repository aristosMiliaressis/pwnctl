using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Commands;
using pwnwrk.infra.Persistence;
using pwnwrk.infra;

using MediatR;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class RunSqlQueryCommandHandler : IRequestHandler<RunSqlQueryCommand, MediatedResponse<object>>
    {
        public async Task<MediatedResponse<object>> Handle(RunSqlQueryCommand command, CancellationToken cancellationToken)
        {
            var queryRunner = new QueryRunner();

            var json = await queryRunner.RunAsync(command.Query);

            return MediatedResponse<object>.Success(PwnContext.Serializer.Deserialize<object>(json));
        }
    }
}