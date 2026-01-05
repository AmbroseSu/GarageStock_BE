namespace DataAccess.DTO.Request;

public class SignInRequest
{
    public string EmailOrUsername { get; set; }
    public string Password { get; set; }
    public string FcmToken { get; set; }
}