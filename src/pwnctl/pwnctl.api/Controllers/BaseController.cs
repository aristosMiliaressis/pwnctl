namespace pwnctl.api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using pwnctl.dto.Api;
using pwnwrk.infra.MediatR;

public abstract class BaseController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    protected ActionResult CreateResponse(MediatorResult result)
    {
        var response = new ApiResponse(result);

        if (result.IsSuccess)
            return Ok(response);

        int status = response.Errors.MaxBy(err => err.Code).ToStatusCode();

        return StatusCode(status, response);
    }
}