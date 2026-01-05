using DataAccess.DTO.Request;
using DataAccess.DTO.Response;

namespace Service;

public interface IAuthenticationService
{
    Task<ResponseDto> SignIn(SignInRequest signInRequest);
}