using AntiClown.Api.Core.Achievements.Domain;
using AntiClown.Api.Core.Achievements.Repositories;
using AutoMapper;

namespace AntiClown.Api.Core.Achievements.Mappings;

public class AchievementsMapperProfile : Profile
{
    public AchievementsMapperProfile()
    {
        CreateMap<Achievement, AchievementStorageElement>().ReverseMap();
        CreateMap<NewAchievement, Achievement>().ForMember(
            a => a.Id,
            cfg => cfg.MapFrom(_ => Guid.NewGuid())
        );

        CreateMap<UserAchievement, UserAchievementStorageElement>().ReverseMap();
    }
}
