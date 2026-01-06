using BusinessObject;
using DataAccess;
using DataAccess.DAO;

namespace Repository.Impl;

public class CategoryRepository : ICategoryRepository
{
    private readonly BaseDao<Categories> _categoryDao;
    
    public CategoryRepository(GarageStockDbContext context)
    {
        _categoryDao = new BaseDao<Categories>(context ?? throw new ArgumentNullException(nameof(context)));
    }
    
    public async Task AddAsync(Categories entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _categoryDao.AddAsync(entity);
    }

    public async Task UpdateAsync(Categories entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await _categoryDao.UpdateAsync(entity);
    }
    
    public async Task<(IEnumerable<Categories> Data, int Total)> FindAllCategoriesAsync(int page, int limit)
    {
        return await _categoryDao.FindPagedAsync(u => true, page, limit);
    }
    
    public async Task<(IEnumerable<Categories> Data, int Total)> FindAllCategoriesActiveAsync(int page, int limit)
    {
        return await _categoryDao.FindPagedAsync(u => u.IsDeleted == false, page, limit);
    }
    
    public async Task<Categories?> FindCategoryByIdAsync(Guid? id, bool? isDeleted = null) 
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        return await _categoryDao.FindByAsync(u => u.CategoryId == id && (isDeleted == null || u.IsDeleted == isDeleted));
    }
    
    public async Task<Categories?> FindCategoryByNameAsync(string name, bool? isDeleted = null)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        return await _categoryDao.FindByAsync(u => u.Name!.ToLower().Equals(name.ToLower()) && (isDeleted == null || u.IsDeleted == isDeleted));
    }
    
    
}