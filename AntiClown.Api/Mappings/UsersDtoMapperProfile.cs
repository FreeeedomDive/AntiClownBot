using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Dto.Users;
using AutoMapper;

namespace AntiClown.Api.Mappings;

public class UsersDtoMapperProfile : Profile
{
    public UsersDtoMapperProfile()
    {
        CreateMap<UserDto, User>().ReverseMap();
        CreateMap<UserFilterDto, UserFilter>();
        CreateMap<NewUserDto, NewUser>();
    }
}