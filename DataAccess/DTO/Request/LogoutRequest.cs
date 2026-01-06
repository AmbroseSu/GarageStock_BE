namespace DataAccess.DTO.Request;

public class LogoutRequest
{
    public string RefreshToken { get; set; } = default!;
}