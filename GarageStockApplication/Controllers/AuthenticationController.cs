using System.Net;
using DataAccess.DTO.Request;
using DataAccess.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Utilities;

namespace GarageStockApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthTokenService _authTokenService;

        public AuthenticationController(IAuthenticationService authenticationService, IAuthTokenService authTokenService)
        {
            _authenticationService = authenticationService;
            _authTokenService = authTokenService;
        }
        
        [HttpPost("sign-in")]
        [AllowAnonymous]
        public async Task<ResponseDto> SiginAsync([FromBody] SignInRequest signInRequest)
        {
            return await _authenticationService.SignIn(signInRequest);
            //return result;
        }
        
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest req)
        {
            var result = await _authTokenService.RefreshAsync(req.RefreshToken);

            if (result == null)
                return Unauthorized(ResponseUtil.Error(ResponseMessages.InvalidOrExpiredRefreshToken, ResponseMessages.Unauthorized, HttpStatusCode.Unauthorized));

            var (access, refresh) = result.Value;

            return Ok(ResponseUtil.GetObject(new RefreshTokenResponse
            {
                Token = access,
                RefreshToken = refresh
            }, ResponseMessages.RefreshTokenSuccess, HttpStatusCode.OK, 1));
        }
        
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ResponseDto> ForgotPassword([FromBody] string email)
        {
            return await _authenticationService.ForgotPasswordAsync(email);
        }
        
        [HttpPost("verify-forgot-otp")]
        [AllowAnonymous]
        public async Task<ResponseDto> VerifyOtp([FromBody] VerifyOtpRequest req)
        {
            return await _authenticationService.VerifyForgotPasswordOtpAsync(req.Email, req.Otp);
        }
        
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ResponseDto> ResetPassword([FromBody] ResetPasswordRequest req)
        {
            return await _authenticationService.ResetPasswordAsync(req);
        }
        
        [Authorize]
        [HttpPost("change-password")]
        public async Task<ResponseDto> ChangePassword(
            [FromBody] ChangePasswordRequest request
        )
        {
            var userId = Guid.Parse(User.FindFirst("userid")!.Value);
            return await _authenticationService.ChangePasswordAsync(userId, request);
        }
        
        [HttpPost("logout")]
        [Authorize]
        public async Task<ResponseDto> Logout([FromBody] LogoutRequest req)
        {
            return await _authTokenService.LogoutAsync(req.RefreshToken);
        }
        
    }
}
