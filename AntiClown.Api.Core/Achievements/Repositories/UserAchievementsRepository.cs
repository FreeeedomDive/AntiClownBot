using AntiClown.Api.Core.Achievements.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api.Core.Achievements.Repositories;

public class UserAchievementsRepository(
    ISqlRepository<UserAchievementStorageElement> sqlRepository,
    IMapper mapper
) : IUserAchievementsRepository
{
    public async Task<UserAchievement[]> GetByUserIdAsync(Guid userId)
    {
        var queryable = await sqlRepository.BuildCustomQueryAsync();
        var result = await queryable.Where(x => x.UserId == userId).ToArrayAsync();
        return mapper.Map<UserAchievement[]>(result);
    }

    public async Task<UserAchievement[]> GetByAchievementIdAsync(Guid achievementId)
    {
        var queryable = await sqlRepository.BuildCustomQueryAsync();
        var result = await queryable.Where(x => x.AchievementId == achievementId).ToArrayAsync();
        return mapper.Map<UserAchievement[]>(result);
    }

    public async Task CreateAsync(UserAchievement userAchievement)
    {
        var storageElement = mapper.Map<UserAchievementStorageElement>(userAchievement);
        await sqlRepository.CreateAsync(storageElement);
    }
}
