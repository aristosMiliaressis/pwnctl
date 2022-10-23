namespace pwnctl.dto.Targets.Commands;

using MediatR;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

public class EditScopeDefCommand : ScopeDefinition, IRequest<MediatorResponse>
{
    public string Target { get; set; }
}