using AntiClown.Api.Core.Achievements.Domain;
using AntiClown.Api.Core.Achievements.Repositories;
using AutoMapper;

namespace AntiClown.Api.Core.Achievements.Services;

public class AchievementsService(
    IAchievementsRepository achievementsRepository,
    IUserAchievementsRepository userAchievementsRepository,
    IMapper mapper
) : IAchievementsService
{
    public async Task<Achievement[]> ReadAllAsync()
    {
        return await achievementsRepository.ReadAllAsync();
    }

    public async Task<Achievement> ReadAsync(Guid id)
    {
        return await achievementsRepository.ReadAsync(id);
    }

    public async Task<Guid> CreateAsync(NewAchievement newAchievement)
    {
        var achievement = mapper.Map<Achievement>(newAchievement);
        await achievementsRepository.CreateAsync(achievement);
        return achievement.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        await achievementsRepository.DeleteAsync(id);
    }

    public async Task<UserAchievement[]> GetByUserIdAsync(Guid userId)
    {
        return await userAchievementsRepository.GetByUserIdAsync(userId);
    }

    public async Task<UserAchievement[]> GetByAchievementIdAsync(Guid achievementId)
    {
        return await userAchievementsRepository.GetByAchievementIdAsync(achievementId);
    }

    public async Task GrantAsync(Guid achievementId, Guid userId, DateTime grantedAt)
    {
        var userAchievement = new UserAchievement
        {
            Id = Guid.NewGuid(),
            AchievementId = achievementId,
            UserId = userId,
            GrantedAt = grantedAt,
        };
        await userAchievementsRepository.CreateAsync(userAchievement);
    }
}
