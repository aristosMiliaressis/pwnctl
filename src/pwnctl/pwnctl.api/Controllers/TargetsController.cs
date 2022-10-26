namespace pwnctl.api.Controllers;

using Microsoft.AspNetCore.Mvc;

using pwnctl.dto.Targets.Commands;
using pwnctl.dto.Targets.Queries;

[ApiController]
[Route("[controller]")]
public class TargetsController : BaseController
{
    [HttpGet]
    public async Task<ActionResult> GetTargets()
    {
        var result = await Mediator.Send(new ListTargetsQuery());

        return CreateResponse(result);    
    }

    [HttpPost]
    public async Task<ActionResult> CreateTarget(CreateTargetCommand command)
    {
        var result = await Mediator.Send(command);
        
        return CreateResponse(result);
    }

    [HttpPatch("{target}")]
    public Task<ActionResult> EditTarget(string target, EditTargetCommand command)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{target}")]
    public Task<ActionResult> DeleteTarget(string target)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{target}/scope")]
    public Task<ActionResult> AddScopeDef(string target, AddScopeDefCommand command)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{target}/scope/{scopeId:int}")]
    public Task<ActionResult> EditScopeDef(string target, int scopeId, EditScopeDefCommand command)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{target}/scope/{scopeId:int}")]
    public Task<ActionResult> DeleteScopeDef(string target, int scopeId)
    {
        throw new NotImplementedException();
    }
}
