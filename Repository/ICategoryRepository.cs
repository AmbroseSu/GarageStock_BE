using BusinessObject;

namespace Repository;

public interface ICategoryRepository
{
    Task AddAsync(Categories entity);
    Task UpdateAsync(Categories entity);
    Task<(IEnumerable<Categories> Data, int Total)> FindAllCategoriesAsync(int page, int limit);
    Task<(IEnumerable<Categories> Data, int Total)> FindAllCategoriesActiveAsync(int page, int limit);
    Task<Categories?> FindCategoryByIdAsync(Guid? id, bool? isDeleted = null);
    Task<Categories?> FindCategoryByNameAsync(string name, bool? isDeleted = null);
}