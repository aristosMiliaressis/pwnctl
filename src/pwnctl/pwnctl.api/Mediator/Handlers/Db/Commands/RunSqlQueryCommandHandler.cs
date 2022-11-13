using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Commands;
using pwnwrk.infra.Persistence;
using pwnwrk.infra;

using MediatR;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class RunSqlQueryCommandHandler : IRequestHandler<RunSqlQueryCommand, MediatedResponse<List<object>>>
    {
        public async Task<MediatedResponse<List<object>>> Handle(RunSqlQueryCommand command, CancellationToken cancellationToken)
        {
            var queryRunner = new QueryRunner();

            var json = await queryRunner.RunAsync(command.Query);

            var result = PwnContext.Serializer.Deserialize<List<object>>(json);

            return MediatedResponse<List<object>>.Success(result);
        }
    }
}