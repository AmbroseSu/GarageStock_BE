using BusinessObject;

namespace Repository;

public interface IVerificationOtpRepository
{
    Task AddAsync(VerificationOtp entity);
    Task UpdateAsync(VerificationOtp entity);
    Task<IEnumerable<VerificationOtp>> FindAllByUserIdAsync(Guid? userId);
    Task<VerificationOtp?> GetLatestActiveOtpAsync(Guid userId);
}