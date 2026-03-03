using AntiClown.Api.Core.Achievements.Domain;
using AntiClown.Api.Dto.Achievements;
using AutoMapper;

namespace AntiClown.Api.Mappings;

public class AchievementsDtoMapperProfile : Profile
{
    public AchievementsDtoMapperProfile()
    {
        CreateMap<Achievement, AchievementDto>()
            .ForMember(dto => dto.Logo, cfg => cfg.MapFrom(src => Convert.ToBase64String(src.Logo)));

        CreateMap<NewAchievementDto, NewAchievement>()
            .ForMember(a => a.Logo, cfg => cfg.MapFrom(dto => Convert.FromBase64String(dto.Logo)));

        CreateMap<UserAchievement, UserAchievementDto>().ReverseMap();
    }
}
