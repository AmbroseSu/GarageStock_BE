using DataAccess.DTO;
using DataAccess.DTO.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace GarageStockApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all-categories")]
        public async Task<ResponseDto> GetAllCategoriesAsync([FromQuery] int page = 1,[FromQuery] int limit = 10)
        {
            return await _categoryService.GetAllAsync(page, limit);
        }

        [HttpGet("get-all-active-categories")]
        public async Task<ResponseDto> GetAllActiveCategoriesAsync([FromQuery] int page = 1,[FromQuery] int limit = 10)
        {
            return await _categoryService.GetAllActiveAsync(page, limit);
        }
        
        [HttpPost("create-category")]
        public async Task<ResponseDto> CreateCategoryAsync([FromBody] CategoryDto request)
        {
            return await _categoryService.CreateAsync(request);
        }
        
        [HttpPost("update-category")]
        public async Task<ResponseDto> UpdateCategoryAsync([FromBody] CategoryDto request)
        {
            return await _categoryService.UpdateAsync(request);
        }
        
        [HttpPost("delete-category/{id}")]
        public async Task<ResponseDto> DeleteCategoryAsync([FromRoute] Guid id)
        {
            return await _categoryService.DeleteAsync(id);
        }
        
        [HttpPost("active-category/{id}")]
        public async Task<ResponseDto> ActiveCategoryAsync([FromRoute] Guid id)
        {
            return await _categoryService.ActiveAsync(id);
        }
        
    }
}
