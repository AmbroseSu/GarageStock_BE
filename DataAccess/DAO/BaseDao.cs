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
        Func<IQueryable<T>, IQueryable<T>> include = null,
        Func<IQueryable<T>, IQueryable<T>> orderBy = null)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        var query = _context.Set<T>().Where(predicate);
        if (include != null) query = include(query);
        if (orderBy != null) query = orderBy(query);

        return await query.ToListAsync();
        //using var context = new DocumentManagementSystemDbContext();
        //return await _context.Set<T>().Where(predicate).ToListAsync();
    }
    
    public async Task<(IEnumerable<T> Data, int Total)> FindPagedAsync(
        Expression<Func<T, bool>> predicate,
        int page,
        int limit,
        Func<IQueryable<T>, IQueryable<T>> include = null,
        Func<IQueryable<T>, IQueryable<T>> orderBy = null)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        if (page <= 0) page = 1;
        if (limit <= 0) limit = 10;

        IQueryable<T> query = _context.Set<T>().Where(predicate);

        if (include != null)
            query = include(query);

        var total = await query.CountAsync();

        if (orderBy != null)
            query = orderBy(query);

        var data = await query
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (data, total);
    }

    public async Task<T?> FindByAsync(Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>> include = null,
        Func<IQueryable<T>, IQueryable<T>> orderBy = null)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        var query = _context.Set<T>().Where(predicate);
        if (include != null) query = include(query);
        if (orderBy != null) query = orderBy(query);

        return await query.FirstOrDefaultAsync();
        //using var context = new DocumentManagementSystemDbContext();
        //return await _context.Set<T>().Where(predicate).FirstOrDefaultAsync();
    }
}