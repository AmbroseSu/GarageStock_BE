

using BusinessObject;

namespace Service;

public interface IJwtService
{
    string GenerateToken(Users user);
    string GenerateRefreshToken(Users user, Dictionary<string, object> extraClaims);
}