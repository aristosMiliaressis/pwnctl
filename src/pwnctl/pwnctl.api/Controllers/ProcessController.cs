namespace pwnctl.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using pwnctl.dto.Process.Commands;

[ApiController]
[Route("[controller]")]
public class ProcessController : BaseController
{
    [HttpPost]
    public async Task<ActionResult> ProcessAssets(ProcessAssetsCommand command)
    {
        var result = await Mediator.Send(command);

        return CreateResponse(result);
    }
}
