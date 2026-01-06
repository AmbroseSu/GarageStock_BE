using DataAccess.DTO.Request;
using DataAccess.DTO.Response;

namespace Service;

public interface IAuthenticationService
{
    Task<ResponseDto> SignIn(SignInRequest signInRequest);
    Task<ResponseDto> ForgotPasswordAsync(string email);
    Task<ResponseDto> VerifyForgotPasswordOtpAsync(string email, string otp);
    Task<ResponseDto> ResetPasswordAsync(ResetPasswordRequest request);
    Task<ResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
}