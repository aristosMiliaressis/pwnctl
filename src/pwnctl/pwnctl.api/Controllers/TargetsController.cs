namespace pwnctl.api.Controllers;

using Microsoft.AspNetCore.Mvc;

using pwnctl.api.Attributes;
using pwnctl.dto.Targets.Commands;
using pwnctl.dto.Targets.Queries;

[ApiController]
[Route("[controller]")]
public sealed class TargetsController : Controller
{
    [HttpEndpoint<ListTargetsQuery>]
    public async Task<ActionResult> GetTargets()
    {
        var result = await Mediator.Send(new ListTargetsQuery());

        return CreateResponse(result);    
    }

    [HttpEndpoint<CreateTargetCommand>]
    public async Task<ActionResult> CreateTarget(CreateTargetCommand command)
    {
        var result = await Mediator.Send(command);
        
        return CreateResponse(result);
    }

    [HttpEndpoint<EditTargetCommand>]
    public async Task<ActionResult> EditTarget(string target, EditTargetCommand command)
    {
        command.Target = target;

        var result = await Mediator.Send(command);

        return CreateResponse(result);
    }

    [HttpEndpoint<DeleteTargetCommand>]
    public async Task<ActionResult> DeleteTarget(string target)
    {
        var command = new DeleteTargetCommand
        {
            Target = target
        };

        var result = await Mediator.Send(command);

        return CreateResponse(result);    
    }

    [HttpEndpoint<AddScopeDefCommand>]
    public Task<ActionResult> AddScopeDef(string target, AddScopeDefCommand command)
    {
        throw new NotImplementedException();
    }

    [HttpEndpoint<EditScopeDefCommand>]
    public Task<ActionResult> EditScopeDef(string target, string scope, EditScopeDefCommand command)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{target}/scope/{scope}")]
    public Task<ActionResult> DeleteScopeDef(string target, string scope)
    {
        throw new NotImplementedException();
    }
}
