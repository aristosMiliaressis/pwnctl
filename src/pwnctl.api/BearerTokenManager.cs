namespace pwnctl.api;

using pwnctl.dto.Auth;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using pwnctl.app;
using System.Text;
using Microsoft.AspNetCore.Identity;
using pwnctl.app.Users.Entities;
using pwnctl.kernel.Extensions;
using pwnctl.kernel;

public class BearerTokenManager
{
    private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public BearerTokenManager(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<TokenGrantResponse> Grant(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
            return null;

        return await Grant(user);
    }

    public async Task<TokenGrantResponse> Grant(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, SystemTime.UtcNow().ToEpochTime().ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PwnInfraContext.Config.Api.HMACSecret));

        var token = new JwtSecurityToken(
                expires: SystemTime.UtcNow().AddMinutes(PwnInfraContext.Config.Api.AccessTimeoutMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        var tokenHandler = new JwtSecurityTokenHandler();

        var refreshTokenBytes = new byte[32];
        _rng.GetBytes(refreshTokenBytes);

        var response = new TokenGrantResponse
        {
            AccessToken = tokenHandler.WriteToken(token),
            RefreshToken = Convert.ToBase64String(refreshTokenBytes)
        };

        user.RefreshToken = response.RefreshToken;
        await _userManager.UpdateAsync(user);

        return response;
    }

    public async Task<TokenGrantResponse> Refresh(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);

        var iat = principal.Claims.First(c => c.Type == JwtRegisteredClaimNames.Iat);
        var expiration = int.Parse(iat.Value) + (PwnInfraContext.Config.Api.RefreshTimeoutHours * 3600);

        var username = principal.Identity.Name;
        var user = await _userManager.FindByNameAsync(username);
        if (user.RefreshToken != refreshToken || expiration <= SystemTime.UtcNow().ToEpochTime())
            return null;

        var response = await Grant(user);

        user.RefreshToken = response.RefreshToken;
        await _userManager.UpdateAsync(user);

        return response;
    }

    public async Task Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PwnInfraContext.Config.Api.HMACSecret)),
            ValidateLifetime = false,
            ValidAlgorithms = new List<string> { SecurityAlgorithms.HmacSha256 }
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        return principal;
    }
}
