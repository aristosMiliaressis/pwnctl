namespace pwnctl.dto.Targets.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

using MediatR;

public class AddScopeDefCommand : ScopeDefinition, IRequest<MediatorResponse>
{
    public string Target { get; set; }
}