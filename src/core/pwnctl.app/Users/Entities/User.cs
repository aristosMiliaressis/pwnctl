using Microsoft.AspNetCore.Identity;
using pwnctl.app.Users.Enums;

namespace pwnctl.app.Users.Entities;

public sealed class User : IdentityUser
{
    public UserRole Role { get; set; }

    public string? RefreshToken { get; set; }
}
