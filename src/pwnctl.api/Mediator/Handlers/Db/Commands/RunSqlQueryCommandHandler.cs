using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Commands;
using pwnctl.infra.Persistence;
using pwnctl.infra.Logging;
using pwnctl.infra;

using MediatR;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class RunSqlQueryCommandHandler : IRequestHandler<RunSqlQueryCommand, MediatedResponse<List<object>>>
    {
        public async Task<MediatedResponse<List<object>>> Handle(RunSqlQueryCommand command, CancellationToken cancellationToken)
        {
            var queryRunner = new QueryRunner();

            try 
            {
                var json = await queryRunner.RunAsync(command.Query);

                var result = PwnContext.Serializer.Deserialize<List<object>>(json);

                return MediatedResponse<List<object>>.Success(result);
            }
            catch (Exception ex)
            {
                PwnContext.Logger.Error(ex.ToRecursiveExInfo());
                return MediatedResponse<List<object>>.Error("{0}", ex.Message);
            }
        }
    }
}