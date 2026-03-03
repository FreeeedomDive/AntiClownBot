using AntiClown.Entertainment.Api.Core.MinecraftAuth.Domain;
using AntiClown.Entertainment.Api.Dto.MinecraftAuth;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Mappings;

public class MinecraftAuthProfile : Profile
{
    public MinecraftAuthProfile()
    {
        CreateMap<AuthResponseDto, AuthResponse>()
            .ForMember(
                x => x.UserId,
                x =>
                    x.MapFrom(y => y.UserUUID)
            )
            .ReverseMap();
        CreateMap<HasJoinedResponseDto, HasJoinedResponse>()
            .ForMember(
                x => x.UserId,
                x =>
                    x.MapFrom(y => y.UserUUID)
            )
            .ReverseMap();
        CreateMap<ProfileResponseDto, ProfileResponse>().ReverseMap();
        CreateMap<ProfilesResponseDto, ProfilesResponse>().ReverseMap();
        CreateMap<RegistrationStatusDto, RegistrationStatus>().ReverseMap();
    }
}