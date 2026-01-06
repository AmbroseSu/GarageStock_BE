using System.Net;
using BusinessObject;
using DataAccess.DTO.Response;
using Microsoft.Extensions.Configuration;
using Repository;
using Service.Utilities;

namespace Service.Impl;

public class AuthTokenService : IAuthTokenService
{
    private readonly IRedisService _redis;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _uow;

    public AuthTokenService(IRedisService redis, IJwtService jwtService, IConfiguration config, IUnitOfWork uow)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }
    
    private string Prefix => _config["Redis:InstancePrefix"] ?? "garagestock:";
    private int RefreshDays => int.Parse(_config["Jwt:RefreshTokenDays"] ?? "30");
    
    private string RtKey(string tokenHash) => $"{Prefix}rt:{tokenHash}";

    public async Task<(string accessToken, string refreshToken)> IssueAsync(Users user)
    {
        var accessToken = _jwtService.GenerateToken(user);

        var refreshToken = TokenUtil.GenerateRefreshToken();
        var refreshHash = TokenUtil.Sha256Base64(refreshToken);

        var ttl = TimeSpan.FromDays(RefreshDays);

        // ❗ revoke old refresh token (nếu có)
        var userKey = $"RT:user:{user.UserId}";
        var oldHash = await _redis.GetStringAsync(userKey);
        if (!string.IsNullOrEmpty(oldHash))
        {
            await _redis.DeleteAsync(RtKey(oldHash));
        }

        // store new token (2 chiều)
        await _redis.SetStringAsync(RtKey(refreshHash), user.UserId.ToString(), ttl);
        await _redis.SetStringAsync(userKey, refreshHash, ttl);

        return (accessToken, refreshToken);
    }

    public async Task<(string accessToken, string refreshToken)?> RefreshAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken)) return null;

        var hash = TokenUtil.Sha256Base64(refreshToken);

        var userIdStr = await _redis.GetStringAsync(RtKey(hash));
        if (string.IsNullOrEmpty(userIdStr)) return null;

        if (!Guid.TryParse(userIdStr, out var userId)) return null;

        // 🔐 ensure this is the active refresh token of user
        var userKey = $"RT:user:{userId}";
        var activeHash = await _redis.GetStringAsync(userKey);
        if (activeHash != hash) return null;

        var user = await _uow.UserUOW.FindUserByIdAsync(userId);
        if (user == null || user.IsDeleted || !user.IsActive) return null;

        // rotate
        await _redis.DeleteAsync(RtKey(hash));
        await _redis.DeleteAsync(userKey);

        // issue new pair
        return await IssueAsync(user);
    }

    public async Task<ResponseDto> LogoutAsync(string refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return ResponseUtil.Error(
                    ResponseMessages.RefreshTokenIsRequired,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            var hash = TokenUtil.Sha256Base64(refreshToken);

            // 1️⃣ Lấy userId từ refresh token
            var userIdStr = await _redis.GetStringAsync(RtKey(hash));

            // ⚠️ Không leak token tồn tại hay không
            if (string.IsNullOrEmpty(userIdStr))
                return ResponseUtil.GetObject(
                    ResponseMessages.LogoutSuccessfully,
                    ResponseMessages.UpdateSuccessfully,
                    HttpStatusCode.OK,
                    0
                );

            // 2️⃣ Xóa refresh token
            await _redis.DeleteAsync(RtKey(hash));

            // 3️⃣ Xóa mapping active token của user
            var userKey = $"RT:user:{userIdStr}";
            await _redis.DeleteAsync(userKey);

            return ResponseUtil.GetObject(
                ResponseMessages.LogoutSuccessfully,
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
    
}