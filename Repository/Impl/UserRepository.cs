using BusinessObject;
using DataAccess;
using DataAccess.DAO;

namespace Repository.Impl;

public class UserRepository : IUserRepository
{
    private readonly BaseDao<Users> _userDao;
    
    public UserRepository(GarageStockDbContext context)
    {
        _userDao = new BaseDao<Users>(context ?? throw new ArgumentNullException(nameof(context)));
    }
    
    public async Task AddAsync(Users entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _userDao.AddAsync(entity);
    }

    public async Task UpdateAsync(Users entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _userDao.UpdateAsync(entity);
    }
    
    public async Task<Users?> FindUserByEmailAsync(string email)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));
        return await _userDao.FindByAsync(u => u.Email!.ToLower().Equals(email.ToLower()));
    }
    
    public async Task<Users?> FindUserByUsernameAsync(string username)
    {
        if (username == null) throw new ArgumentNullException(nameof(username));
        return await _userDao.FindByAsync(u => u.Username!.ToLower().Equals(username.ToLower()));
    }
    
    public async Task<Users?> FindUserByIdAsync(Guid? id)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        return await _userDao.FindByAsync(u => u.UserId == id);
    }
    
    public async Task<IEnumerable<Users>> FindAllUserAsync()
    {
        return await _userDao.FindAsync(u => true);
    }
    
}