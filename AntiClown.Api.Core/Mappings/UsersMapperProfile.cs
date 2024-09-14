using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Repositories;
using AutoMapper;

namespace AntiClown.Api.Core.Mappings;

public class UsersMapperProfile : Profile
{
    public UsersMapperProfile()
    {
        CreateMap<User, UserStorageElement>().ReverseMap();
        CreateMap<NewUser, User>().ForMember(
            user => user.Id,
            cfg => cfg.MapFrom(_ => Guid.NewGuid())
        )
        .ForMember(
            user => user.TelegramId,
            cfg => cfg.MapFrom(_ => (long?)null)
        );
    }
}