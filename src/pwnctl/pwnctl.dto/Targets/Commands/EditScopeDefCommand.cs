namespace pwnctl.dto.Targets.Commands;

using MediatR;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

public class EditScopeDefCommand : ScopeDefinition, IRequest<MediatorResult>
{
    public string Target { get; set; }
}