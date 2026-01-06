using System.Net;
using AutoMapper;
using BusinessObject;
using BusinessObject.Enums;
using DataAccess.DTO;
using DataAccess.DTO.Request;
using DataAccess.DTO.Response;
using Microsoft.Extensions.Configuration;
using Repository;
using Service.Utilities;

namespace Service.Impl;

public class AuthenticationService : IAuthenticationService
{
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly ISendMailService _sendMailService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthTokenService _authTokenService;
    private readonly IRedisService _redis;
    private readonly IConfiguration _config;
    
    public AuthenticationService(IJwtService jwtService, IEmailService emailService, IMapper mapper, IUnitOfWork unitOfWork, IAuthTokenService authTokenService, ISendMailService sendMailService, IRedisService redis, IConfiguration config)
    {
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _authTokenService = authTokenService;
        _sendMailService = sendMailService;
        _redis = redis;
        _config = config;
    }
    
    private string Prefix => _config["Redis:InstancePrefix"] ?? "garagestock:";

    public async Task<ResponseDto> SignIn(SignInRequest signInRequest)
    {
        try
        {
            var user = await _unitOfWork.UserUOW.FindUserByEmailAsync(signInRequest.EmailOrUsername) 
                       ?? await _unitOfWork.UserUOW.FindUserByUsernameAsync(signInRequest.EmailOrUsername);
            if (user == null || !BCrypt.Net.BCrypt.Verify(signInRequest.Password, user.Password))
                return ResponseUtil.Error(ResponseMessages.EmailOrUsernameNotExists + " or " + ResponseMessages.PasswordNotExists,
                    ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);
            if (user.IsDeleted)
                return ResponseUtil.Error(ResponseMessages.UserHasDeleted, ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest);

            if (!signInRequest.FcmToken.Equals(user.FcmToken) && !signInRequest.FcmToken.Equals("string"))
            {
                user.FcmToken = signInRequest.FcmToken;
            }

            //var jwtToken = _jwtService.GenerateToken(user);
            //var refreshToken = _jwtService.GenerateRefreshToken(user, new Dictionary<string, object>());
            
            var (jwtToken, refreshToken) = await _authTokenService.IssueAsync(user);
            var jwtAuthResponse = new JwtAuthenticationResponse();
            var userDto = _mapper.Map<UserDto>(user);
            
            jwtAuthResponse.UserDto = userDto;
            jwtAuthResponse.Token = jwtToken;
            jwtAuthResponse.RefreshToken = refreshToken;
            await _unitOfWork.UserUOW.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return ResponseUtil.GetObject(jwtAuthResponse, ResponseMessages.CreatedSuccessfully, HttpStatusCode.Created,
                0);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<ResponseDto> RefreshTokenAsync(RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return ResponseUtil.Error(
                ResponseMessages.RefreshRequired,
                ResponseMessages.OperationFailed,
                HttpStatusCode.BadRequest);

        var result = await _authTokenService.RefreshAsync(request.RefreshToken);

        if (result == null)
            return ResponseUtil.Error(
                ResponseMessages.InvalidOrExpiredRefreshToken,
                ResponseMessages.Unauthorized,
                HttpStatusCode.Unauthorized);

        var (accessToken, refreshToken) = result.Value;
        
        var refreshTokenResponse = new RefreshTokenResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken
        };

        return ResponseUtil.GetObject(
            refreshTokenResponse,
            ResponseMessages.GetSuccessfully,
            HttpStatusCode.OK,
            0);
    }
    
    public async Task<ResponseDto> ForgotPasswordAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return ResponseUtil.Error(
                    ResponseMessages.EmailIsRequired,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            email = email.Trim();

            var user = await _unitOfWork.UserUOW.FindUserByEmailAsync(email);

            // ⚠️ KHÔNG leak email tồn tại hay không
            if (user == null)
                return ResponseUtil.GetObject(
                ResponseMessages.IfEmailExists,
                ResponseMessages.GetSuccessfully,
                HttpStatusCode.OK,
                0);

            // 1️⃣ Invalidate OTP cũ (nếu có)
            var oldOtps = await _unitOfWork.VerificationOtpUOW.FindAllByUserIdAsync(user.UserId);
            Guid sendMailId;
            await using var tx = await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                foreach (var otp in oldOtps)
                {
                    otp.IsDeleted = true;
                }
            
                // 2️⃣ Tạo OTP mới
                var otpCode = GenerateOtp();

                var otpEntity = new VerificationOtp(otpCode, user.UserId);

                await _unitOfWork.VerificationOtpUOW.AddAsync(otpEntity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // 3️⃣ Gửi mail OTP
                sendMailId = await _sendMailService.EnqueueForgotPasswordEmail(user, otpCode);
            }
            catch (Exception e)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
            
            await _sendMailService.EnqueueJobAsync(sendMailId);

            return ResponseUtil.GetObject(
                ResponseMessages.IfEmailExists,
                ResponseMessages.GetSuccessfully,
                HttpStatusCode.OK,
                0);
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(
                ex.Message,
                ResponseMessages.OperationFailed,
                HttpStatusCode.InternalServerError
            );
        }
    }
    
    
    public async Task<ResponseDto> VerifyForgotPasswordOtpAsync(string email, string otp)
    {
        try
        {
            var user = await _unitOfWork.UserUOW.FindUserByEmailAsync(email);
            if (user == null)
                return ResponseUtil.Error(ResponseMessages.InvalidOtp, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);

            var verificationOtp = await _unitOfWork.VerificationOtpUOW.GetLatestActiveOtpAsync(user.UserId);

            if (verificationOtp == null)
                return ResponseUtil.Error(ResponseMessages.OtpExpired, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);

            // 1️⃣ Check expire
            if (verificationOtp.ExpirationTime < DateTime.Now)
            {
                verificationOtp.IsDeleted = true;
                await _unitOfWork.SaveChangesAsync();

                return ResponseUtil.Error(ResponseMessages.OtpExpired, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);
            }

            // 2️⃣ Check attempt
            if (verificationOtp.AttemptCount >= 5)
            {
                verificationOtp.IsDeleted = true;
                await _unitOfWork.SaveChangesAsync();

                return ResponseUtil.Error(ResponseMessages.OtpBlocked, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);
            }

            // 3️⃣ Check OTP
            if (verificationOtp.OtpCode != otp)
            {
                verificationOtp.AttemptCount++;
                await _unitOfWork.SaveChangesAsync();

                return ResponseUtil.Error(ResponseMessages.InvalidOtp, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);
            }
            
            var resetToken = TokenUtil.GenerateRefreshToken();
            var resetTokenHash = TokenUtil.Sha256Base64(resetToken);

            // 4️⃣ OTP đúng
            verificationOtp.IsTrue = true;
            verificationOtp.IsDeleted = true;
            
            var userKey = $"reset-password:user:{user.UserId}";
            var oldHash = await _redis.GetStringAsync(userKey);
            if (!string.IsNullOrEmpty(oldHash))
            {
                await _redis.DeleteAsync($"reset-password:{oldHash}");
            }

            // ✅ Lưu token mới (2 key)
            var ttl = TimeSpan.FromMinutes(5);

            await _redis.SetStringAsync(
                $"reset-password:{resetTokenHash}",
                user.UserId.ToString(),
                ttl
            );

            await _redis.SetStringAsync(
                userKey,
                resetTokenHash,
                ttl
            );
            
            await _unitOfWork.SaveChangesAsync();
            
            var response = new ResetTokenResponse()
            {
                ResetToken = resetToken
            };

            return ResponseUtil.GetObject(
                response,
                ResponseMessages.OtpVerified,
                HttpStatusCode.OK,
                0);
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(
                ex.Message,
                ResponseMessages.OperationFailed,
                HttpStatusCode.InternalServerError
            );
        }
    }
    
    public async Task<ResponseDto> ResetPasswordAsync(ResetPasswordRequest request)
    {
        if (request == null)
            return ResponseUtil.Error(
                ResponseMessages.ValueNull,
                ResponseMessages.OperationFailed,
                HttpStatusCode.BadRequest
            );

        if (string.IsNullOrWhiteSpace(request.ResetToken))
            return ResponseUtil.Error(
                ResponseMessages.ResetTokenIsRequired,
                ResponseMessages.OperationFailed,
                HttpStatusCode.BadRequest
            );

        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return ResponseUtil.Error(
                ResponseMessages.PasswordIsRequired,
                ResponseMessages.OperationFailed,
                HttpStatusCode.BadRequest
            );

        if (string.IsNullOrWhiteSpace(request.ConfirmPassword))
            return ResponseUtil.Error(
                ResponseMessages.ConfirmPasswordIsRequired,
                ResponseMessages.OperationFailed,
                HttpStatusCode.BadRequest
            );

        // 1️⃣ Check confirm password
        if (request.NewPassword != request.ConfirmPassword)
            return ResponseUtil.Error(
                ResponseMessages.PasswordConfirmNotMatch,
                ResponseMessages.OperationFailed,
                HttpStatusCode.BadRequest
            );

        // 2️⃣ (Optional nhưng rất nên) Check password strength
        if (!IsStrongPassword(request.NewPassword))
            return ResponseUtil.Error(
                ResponseMessages.PasswordTooWeak,
                ResponseMessages.OperationFailed,
                HttpStatusCode.BadRequest
            );
        
        
        var tokenHash = TokenUtil.Sha256Base64(request.ResetToken);

        var userIdStr = await _redis.GetStringAsync($"reset-password:{tokenHash}");
        if (string.IsNullOrEmpty(userIdStr))
            return ResponseUtil.Error(ResponseMessages.ResetTokenInvalidOrExpired, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);

        var userId = Guid.Parse(userIdStr);

        // 2) (Khuyên) check token đang active của user có đúng tokenHash không
        var userKey = $"reset-password:user:{userId}";
        var activeHash = await _redis.GetStringAsync(userKey);
        if (string.IsNullOrEmpty(activeHash) || activeHash != tokenHash)
            return ResponseUtil.Error(ResponseMessages.ResetTokenInvalidOrExpired, ResponseMessages.OperationFailed, HttpStatusCode.BadRequest);
        var user = await _unitOfWork.UserUOW.FindUserByIdAsync(userId);

        if (user == null)
            return ResponseUtil.Error(
                ResponseMessages.UserNotFound,
                ResponseMessages.OperationFailed,
                HttpStatusCode.BadRequest
            );

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.Now;

        await _unitOfWork.SaveChangesAsync();

        // ❗ invalidate token
        await _redis.DeleteAsync($"reset-password:{tokenHash}");
        await _redis.DeleteAsync(userKey);

        return ResponseUtil.GetObject(
            ResponseMessages.PasswordResetSuccessfully,
            ResponseMessages.UpdateSuccessfully,
            HttpStatusCode.OK,
            0
        );
    }

    public async Task<ResponseDto> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        try
        {
            // 0️⃣ Validate input
            if (request == null)
                return ResponseUtil.Error(
                    ResponseMessages.ValueNull,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            if (string.IsNullOrWhiteSpace(request.OldPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
                return ResponseUtil.Error(
                    ResponseMessages.ValueNull,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            // 1️⃣ Check confirm password
            if (request.NewPassword != request.ConfirmPassword)
                return ResponseUtil.Error(
                    ResponseMessages.PasswordConfirmNotMatch,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            // 2️⃣ Load user
            var user = await _unitOfWork.UserUOW.FindUserByIdAsync(userId);
            if (user == null)
                return ResponseUtil.Error(
                    ResponseMessages.UserNotFound,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            // 3️⃣ Check old password
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.Password))
                return ResponseUtil.Error(
                    ResponseMessages.OldPasswordInvalid,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            // 4️⃣ New password must differ
            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.Password))
                return ResponseUtil.Error(
                    ResponseMessages.NewPasswordMustBeDifferent,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            // 5️⃣ (Optional) Check password strength
            if (!IsStrongPassword(request.NewPassword))
                return ResponseUtil.Error(
                    ResponseMessages.PasswordTooWeak,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            // 6️⃣ Update password
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();

            // 7️⃣ Revoke all refresh tokens (IMPORTANT)
            var userKey = $"RT:user:{userId}";
            var oldHash = await _redis.GetStringAsync(userKey);

            if (!string.IsNullOrEmpty(oldHash))
            {
                await _redis.DeleteAsync($"{Prefix}rt:{oldHash}");
                await _redis.DeleteAsync(userKey);
            }

            return ResponseUtil.GetObject(
                ResponseMessages.PasswordChangedSuccessfully,
                ResponseMessages.UpdateSuccessfully,
                HttpStatusCode.OK,
                0
            );
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(
                ex.Message,
                ResponseMessages.OperationFailed,
                HttpStatusCode.InternalServerError
            );
        }
    }
    
    private static string GenerateOtp()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }
    
    private bool IsStrongPassword(string password)
    {
        if (password.Length < 8) return false;

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);

        return hasUpper && hasLower && hasDigit;
    }
}