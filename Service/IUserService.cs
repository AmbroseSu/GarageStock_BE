using DataAccess.DTO.Request;
using DataAccess.DTO.Response;

namespace Service;

public interface IUserService
{
    Task<ResponseDto> CreateUser(UserRequest userRequest);
}