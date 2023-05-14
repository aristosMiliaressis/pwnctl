namespace pwnctl.api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pwnctl.dto.Auth;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly BearerTokenManager _bearerManager;

    public AuthController(BearerTokenManager bearerManager)
    {
        _bearerManager = bearerManager;
    }

    [HttpPost("grant")]
    public async Task<ActionResult<TokenGrantResponse>> Grant(AccessTokenRequestModel request)
    {
        var response = await _bearerManager.Grant(request.Username, request.Password);
        if (response == null)
            return Unauthorized();

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenGrantResponse>> Refresh(RefreshTokenRequestModel request)
    {
        var response = await _bearerManager.Refresh(request.AccessToken, request.RefreshToken);
        if (response == null)
            return Unauthorized();

        return Ok(response);
    }

    [HttpPost("revoke"), Authorize]
    public async Task<ActionResult> Revoke()
    {
        await _bearerManager.Revoke(User.Identity.Name);

        return NoContent();
    }
}
