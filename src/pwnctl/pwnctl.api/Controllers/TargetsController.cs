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
        var response = await Mediator.Send(new ListTargetsQuery());

        return CreateResponse(response);    
    }

    [HttpPost]
    public async Task<ActionResult> CreateTarget(CreateTargetCommand target)
    {
        var response = await Mediator.Send(target);
        
        return CreateResponse(response);
    }

    [HttpPatch("{target}")]
    public Task<ActionResult> EditTarget(string target, EditTargetCommand edit)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{target}")]
    public Task<ActionResult> DeleteTarget(string target)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{target}/scope")]
    public Task<ActionResult> AddScopeDef(string target, AddScopeDefCommand scope)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("{target}/scope/{scopeId:int}")]
    public Task<ActionResult> EditScopeDef(string target, int scopeId, EditScopeDefCommand scope)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{target}/scope/{scopeId:int}")]
    public Task<ActionResult> DeleteScopeDef(string target, int scopeId)
    {
        throw new NotImplementedException();
    }
}
