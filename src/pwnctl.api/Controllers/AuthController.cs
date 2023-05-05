namespace pwnctl.api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pwnctl.dto.Auth;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly BearerTokenManager _bearerManager;

    public AuthController(ILogger<AuthController> logger,BearerTokenManager bearerManager)
    {
        _logger = logger;
        _bearerManager = bearerManager;
    }

    [HttpPost("token")]
    public async Task<ActionResult<TokenGrantResponse>> Token(AccessTokenRequestModel request)
    {
        var succeeded = await _bearerManager.ValidateCreds(request.Username, request.Password);
        if (!succeeded)
            return Unauthorized();

        var response = _bearerManager.Generate(request.Username);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenGrantResponse>> Refrech(RefreshTokenRequestModel request)
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
