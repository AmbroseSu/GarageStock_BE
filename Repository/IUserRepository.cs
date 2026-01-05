using BusinessObject;

namespace Repository;

public interface IUserRepository
{
    Task AddAsync(Users entity);
    Task UpdateAsync(Users entity);
    Task<Users?> FindUserByEmailAsync(string email);
    Task<Users?> FindUserByUsernameAsync(string username);
    Task<Users?> FindUserByIdAsync(Guid? id);
    Task<IEnumerable<Users>> FindAllUserAsync();
}