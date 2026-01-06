using BusinessObject;
using BusinessObject.Enums;
using DataAccess;
using DataAccess.DAO;

namespace Repository.Impl;

public class MailTemplateRepository : IMailTemplateRepository
{
    private readonly BaseDao<MailTemplate> _mailTemplateDao;
    
    public MailTemplateRepository(GarageStockDbContext context)
    {
        _mailTemplateDao = new BaseDao<MailTemplate>(context ?? throw new ArgumentNullException(nameof(context)));
    }
    
    public async Task AddAsync(MailTemplate entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _mailTemplateDao.AddAsync(entity);
    }

    public async Task UpdateAsync(MailTemplate entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _mailTemplateDao.UpdateAsync(entity);
    }
    
    public async Task<MailTemplate?> GetByCodeAsync(MailTemplateCode? templateCode)
    {
        if (templateCode == null) throw new ArgumentNullException(nameof(templateCode));
        return await _mailTemplateDao.FindByAsync(u => u.TemplateCode.Equals(templateCode));
    }
}