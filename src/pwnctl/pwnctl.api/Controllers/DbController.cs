namespace pwnctl.api.Controllers;

using Microsoft.AspNetCore.Mvc;
using pwnwrk.infra.Persistence;

[ApiController]
[Route("[controller]")]
public sealed class DbController : ControllerBase
{
    [HttpPost("/initialize")]
    public async Task<ActionResult> Initialize()
    {
        await DatabaseInitializer.InitializeAsync();

        return Ok();
    }

    [HttpPost("/query")]
    public Task<ActionResult> Query()
    {
        throw new NotImplementedException();
    }
}
