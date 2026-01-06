using BusinessObject;
using DataAccess;
using DataAccess.DAO;

namespace Repository.Impl;

public class VerificationOtpRepository : IVerificationOtpRepository
{
    private readonly BaseDao<VerificationOtp> _verificationOtpDao;
    
    public VerificationOtpRepository(GarageStockDbContext context)
    {
        _verificationOtpDao = new BaseDao<VerificationOtp>(context ?? throw new ArgumentNullException(nameof(context)));
    }
    
    public async Task AddAsync(VerificationOtp entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _verificationOtpDao.AddAsync(entity);
    }

    public async Task UpdateAsync(VerificationOtp entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _verificationOtpDao.UpdateAsync(entity);
    }
    
    public async Task<IEnumerable<VerificationOtp>> FindAllByUserIdAsync(Guid? userId)
    {
        if (userId == null) throw new ArgumentNullException(nameof(userId));
        return await _verificationOtpDao.FindAsync(v => v.UserId == userId && !v.IsDeleted && !v.IsTrue);
    }
    
    public async Task<VerificationOtp?> GetLatestActiveOtpAsync(Guid userId)
    {
        var otps = await _verificationOtpDao.FindAsync(v =>
            v.UserId == userId &&
            !v.IsDeleted &&
            !v.IsTrue
        );

        return otps
            .OrderByDescending(v => v.ExpirationTime)
            .FirstOrDefault();
    }
}