using BusinessObject;

namespace Repository;

public interface ISendMailRepository
{
    Task AddAsync(SendMail entity);
    Task UpdateAsync(SendMail entity);
    Task<SendMail?> GetByIdAsync(Guid? id);
}