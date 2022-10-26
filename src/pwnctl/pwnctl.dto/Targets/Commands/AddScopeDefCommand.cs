namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

using MediatR;

public class AddScopeDefCommand : ScopeDefinition, IRequest<MediatorResult>
{
    public string Target { get; set; }
}