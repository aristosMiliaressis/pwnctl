namespace pwnctl.api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ProcessController : ControllerBase
{
    private readonly ILogger<ProcessController> _logger;

    public ProcessController(ILogger<ProcessController> logger)
    {
        _logger = logger;
    }

    // [HttpPost]
    // public async Task<ActionResult> ProcessAssets()
    // {
    //     throw new NotImplementedException();
    // }
}
