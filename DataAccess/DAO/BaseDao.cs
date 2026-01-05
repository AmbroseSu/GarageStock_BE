using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DAO;

public class BaseDao<T> where T : class
{
    private readonly GarageStockDbContext _context;
    
        public BaseDao(GarageStockDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }


    public async Task AddAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        //using var context = new DocumentManagementSystemDbContext();
        _context.Set<T>().Add(entity);
        //await context.SaveChangesAsync();
    }
    

    public async Task AddRangeAsync(List<T> entities)
    {
        if (entities == null || entities.Count == 0) return;
        await _context.Set<T>().AddRangeAsync(entities);
    }
    
    public async Task DeleteAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        //using var context = new DocumentManagementSystemDbContext();
        _context.Set<T>().Remove(entity);
        //await context.SaveChangesAsync();
    }

    public async Task RemoveRangeAsync(List<T> entities)
    {
        if (entities == null || entities.Count == 0) return;
        _context.Set<T>().RemoveRange(entities);
        //await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        //using var context = new DocumentManagementSystemDbContext();
        _context.Set<T>().Update(entity);
        //await _context.SaveChangesAsync();
    }
    
    public async Task UpdateRangeAsync(List<T> entities)
    {
        if (entities == null || entities.Count == 0) return;
        //using var context = new DocumentManagementSystemDbContext();
        _context.Set<T>().UpdateRange(entities);
        //await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        //using var context = new DocumentManagementSystemDbContext();
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> include = null)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        var query = _context.Set<T>().Where(predicate);
        if (include != null) query = include(query);

        return await query.ToListAsync();
        //using var context = new DocumentManagementSystemDbContext();
        //return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<T?> FindByAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> include = null)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        var query = _context.Set<T>().Where(predicate);
        if (include != null) query = include(query);

        return await query.FirstOrDefaultAsync();
        //using var context = new DocumentManagementSystemDbContext();
        //return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
    }
}