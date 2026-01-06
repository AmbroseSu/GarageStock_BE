namespace DataAccess.DTO.Response;

public class RefreshTokenResponse
{
    public string Token { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}