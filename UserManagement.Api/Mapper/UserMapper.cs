using AutoMapper;
using UserManagement.Api.Data;
using UserManagement.Api.Repositorys;
using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.Api.Mapper;

public class UserMapper : Profile
{
    private UsersDbContext _usersDbContext;
    public UserMapper(UsersDbContext usersDbContext)
    {
        _usersDbContext = usersDbContext;
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id,opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => string.Join(",", src.Roles!.Select(r => r.Name))));
        
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Roles, opt =>opt.MapFrom(src => FindRoles(src.Roles)));
    }

    private List<Role> FindRoles(string? roles)
    {
        var rolesNames = roles?.Split(',').ToList();
        var allRoles = _usersDbContext.Roles.ToList();
        var res =  allRoles.Where(r => r.Name != null && rolesNames != null && rolesNames.Contains(r.Name)).ToList();
        return res;
    }
}
