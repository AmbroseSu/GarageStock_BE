using DataAccess.DTO;
using DataAccess.DTO.Response;

namespace Service;

public interface ICategoryService
{
    Task<ResponseDto> GetAllAsync(int page, int limit, string? search);
    Task<ResponseDto> GetAllActiveAsync(int page, int limit, string? search);
    Task<ResponseDto> CreateAsync(CategoryDto request);
    Task<ResponseDto> UpdateAsync(CategoryDto request);
    Task<ResponseDto> DeleteAsync(Guid id);
    Task<ResponseDto> ActiveAsync(Guid id);
}