using System.Net;
using AutoMapper;
using BusinessObject;
using DataAccess.DTO;
using DataAccess.DTO.Response;
using Repository;
using Service.Utilities;

namespace Service.Impl;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<ResponseDto> GetAllAsync(int page, int limit)
    {
        try
        {
            var (data, total) = await _unitOfWork.CategoryUOW
                .FindAllCategoriesAsync(page, limit);

            var result = _mapper.Map<List<CategoryDto>>(data);

            return ResponseUtil.GetCollection(
                result,
                ResponseMessages.GetSuccessfully,
                HttpStatusCode.OK,
                total,
                page,   
                limit,
                total
            );
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(
                ex.Message,
                ResponseMessages.OperationFailed,
                HttpStatusCode.InternalServerError
            );
        }
    }
    
    public async Task<ResponseDto> GetAllActiveAsync(int page, int limit)
    {
        try
        {
            var (data, total) = await _unitOfWork.CategoryUOW
                .FindAllCategoriesActiveAsync(page, limit);

            var result = _mapper.Map<List<CategoryDto>>(data);

            return ResponseUtil.GetCollection(
                result,
                ResponseMessages.GetSuccessfully,
                HttpStatusCode.OK,
                total,
                page,
                limit,
                total
            );
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(
                ex.Message,
                ResponseMessages.OperationFailed,
                HttpStatusCode.InternalServerError
            );
        }
    }
    
    public async Task<ResponseDto> CreateAsync(CategoryDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResponseUtil.Error(
                    ResponseMessages.NameIsRequired,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            var existed = await _unitOfWork.CategoryUOW.FindCategoryByNameAsync(request.Name.Trim());
            if (existed != null)
                return ResponseUtil.Error(
                    ResponseMessages.NameAlreadyExists,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            var category = new Categories()
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                IsDeleted = false
            };

            await _unitOfWork.CategoryUOW.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            var categoryResult = _mapper.Map<CategoryDto>(category);

            return ResponseUtil.GetObject(
                categoryResult,
                ResponseMessages.CreatedSuccessfully,
                HttpStatusCode.Created, 1);
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(ex.Message, ResponseMessages.OperationFailed, HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ResponseDto> UpdateAsync(CategoryDto request)
    {
        try
        {
            if (!request.CategoryId.HasValue || request.CategoryId == Guid.Empty)
                return ResponseUtil.Error(
                    ResponseMessages.CategoryIdIsRequired,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.BadRequest
                );

            var category = await _unitOfWork.CategoryUOW
                .FindCategoryByIdAsync(request.CategoryId.Value, false);

            if (category == null)
                return ResponseUtil.Error(
                    ResponseMessages.CategoryNotFound,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.NotFound
                );

            var isChanged = false;

            // ===== NAME =====
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var newName = request.Name.Trim();

                // chỉ xử lý nếu khác name hiện tại
                if (!string.Equals(category.Name, newName, StringComparison.OrdinalIgnoreCase))
                {
                    // check trùng với category khác
                    var existed = await _unitOfWork.CategoryUOW.FindCategoryByNameAsync(newName);
                    if (existed != null && existed.CategoryId != category.CategoryId)
                    {
                        return ResponseUtil.Error(
                            ResponseMessages.NameAlreadyExists,
                            ResponseMessages.OperationFailed,
                            HttpStatusCode.BadRequest
                        );
                    }

                    category.Name = newName;
                    isChanged = true;
                }
            }

            // ===== DESCRIPTION =====
            if (request.Description != null)
            {
                // chỉ update nếu khác
                if (!string.Equals(category.Description, request.Description))
                {
                    category.Description = request.Description;
                    isChanged = true;
                }
            }

            // Nếu không có field nào thay đổi
            if (!isChanged)
            {
                return ResponseUtil.GetObject(
                    _mapper.Map<CategoryDto>(category),
                    ResponseMessages.NoDataChanged,
                    HttpStatusCode.OK,
                    1
                );
            }

            await _unitOfWork.SaveChangesAsync();

            return ResponseUtil.GetObject(
                _mapper.Map<CategoryDto>(category),
                ResponseMessages.UpdateSuccessfully,
                HttpStatusCode.OK,
                1
            );
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(
                ex.Message,
                ResponseMessages.OperationFailed,
                HttpStatusCode.InternalServerError
            );
        }
    }
    
    public async Task<ResponseDto> DeleteAsync(Guid id)
    {
        try
        {
            var category = await _unitOfWork.CategoryUOW.FindCategoryByIdAsync(id, false);
            if (category == null)
                return ResponseUtil.Error(
                    ResponseMessages.CategoryNotFound,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.NotFound
                );

            category.IsDeleted = true;

            await _unitOfWork.SaveChangesAsync();

            return ResponseUtil.GetObject(
                ResponseMessages.CategoryDeleted,
                ResponseMessages.DeleteSuccessfully,
                HttpStatusCode.OK, 1);
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(ex.Message, ResponseMessages.OperationFailed, HttpStatusCode.InternalServerError);
        }
    }
    
    public async Task<ResponseDto> ActiveAsync(Guid id)
    {
        try
        {
            var category = await _unitOfWork.CategoryUOW.FindCategoryByIdAsync(id, true);
            if (category == null)
                return ResponseUtil.Error(
                    ResponseMessages.CategoryNotFound,
                    ResponseMessages.OperationFailed,
                    HttpStatusCode.NotFound
                );

            category.IsDeleted = false;

            await _unitOfWork.SaveChangesAsync();

            return ResponseUtil.GetObject(
                ResponseMessages.CategoryActived,
                ResponseMessages.DeleteSuccessfully,
                HttpStatusCode.OK, 1);
        }
        catch (Exception ex)
        {
            return ResponseUtil.Error(ex.Message, ResponseMessages.OperationFailed, HttpStatusCode.InternalServerError);
        }
    }
    
}