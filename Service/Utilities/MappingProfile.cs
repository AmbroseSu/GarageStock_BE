using AutoMapper;
using BusinessObject;
using DataAccess.DTO;
using DataAccess.DTO.Request;

namespace Service.Utilities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Users, UserDto>();
        CreateMap<UserDto, Users>();
        CreateMap<UserRequest, Users>();
        CreateMap<Users, UserRequest>();
        CreateMap<Categories, CategoryDto>();
        CreateMap<CategoryDto, Categories>();
    }
}