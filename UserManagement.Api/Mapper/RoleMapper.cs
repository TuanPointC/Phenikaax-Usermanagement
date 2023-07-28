using AutoMapper;
using UserManagement.Shared.Models;
using UserManagement.Shared.Models.Dtos;

namespace UserManagement.Api.Mapper;

public class RoleMapper : Profile
{
    public RoleMapper()
    {
        CreateMap<Role, RolesDto>()
            .ForMember(dest => dest.Id,opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
    }
}
