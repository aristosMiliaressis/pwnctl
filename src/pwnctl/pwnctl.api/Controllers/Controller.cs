using MediatR;
using Microsoft.AspNetCore.Mvc;
using pwnctl.dto.Mediator;

namespace pwnctl.api.Controllers
{
    public abstract class Controller : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected ActionResult CreateResponse(MediatedResponse result)
        {
            if (result.IsSuccess)
                return Ok(result);

            int status = result.Errors.MaxBy(err => err.Type).ToStatusCode();

            return StatusCode(status, result);
        }
    }
}