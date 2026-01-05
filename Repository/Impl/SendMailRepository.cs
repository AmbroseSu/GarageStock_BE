using BusinessObject;
using DataAccess;
using DataAccess.DAO;

namespace Repository.Impl;

public class SendMailRepository : ISendMailRepository
{
    private readonly BaseDao<SendMail> _sendMailDao;
    
    public SendMailRepository(GarageStockDbContext context)
    {
        _sendMailDao = new BaseDao<SendMail>(context ?? throw new ArgumentNullException(nameof(context)));
    }
    
    public async Task AddAsync(SendMail entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _sendMailDao.AddAsync(entity);
    }

    public async Task UpdateAsync(SendMail entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _sendMailDao.UpdateAsync(entity);
    }
    
    public async Task<SendMail?> GetByIdAsync(Guid? id)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        return await _sendMailDao.FindByAsync(u => u.SendMailId == id);
    }
}