namespace pwnctl.dto.Auth;

public class TokenGrantResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
