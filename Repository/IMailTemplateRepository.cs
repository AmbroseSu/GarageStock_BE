using BusinessObject;
using BusinessObject.Enums;

namespace Repository;

public interface IMailTemplateRepository
{
    Task AddAsync(MailTemplate entity);
    Task UpdateAsync(MailTemplate entity);
    Task<MailTemplate?> GetByCodeAsync(MailTemplateCode? templateCode);
}