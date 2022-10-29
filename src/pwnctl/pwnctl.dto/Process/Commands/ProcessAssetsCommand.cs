namespace pwnctl.dto.Process.Commands;

using pwnwrk.domain.Entities;
using pwnwrk.infra.MediatR;

using MediatR;

public class ProcessAssetsCommand : ScopeDefinition, IRequest<MediatorResult>
{
    public List<string> Assets { get; set; }
}