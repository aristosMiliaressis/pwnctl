namespace pwnctl.api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using pwnctl.api.Models;
using pwnwrk.infra.MediatR;

public abstract class BaseController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

    protected ActionResult CreateResponse(MediatorResponse response)
    {
        var apiResponse = new ApiResponse(response);

        if (response.IsSuccess)
            return Ok(apiResponse);

        return CreateErrorResponse(apiResponse);
    }

    private ActionResult CreateErrorResponse(ApiResponse response)
    {
        switch (response.Errors.Max(err => err.Code))
        {
            case ApiError.ErrorCode.InternalServerError:
            default:
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }
    }
}