using BusinessObject;
using DataAccess.DTO.Response;

namespace Service;

public interface IAuthTokenService
{
    Task<(string accessToken, string refreshToken)> IssueAsync(Users user);
    Task<(string accessToken, string refreshToken)?> RefreshAsync(string refreshToken);
    Task<ResponseDto> LogoutAsync(string refreshToken);
}