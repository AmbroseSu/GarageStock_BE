namespace DataAccess.DTO.Response;

public class JwtAuthenticationResponse
{
    public UserDto UserDto { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}