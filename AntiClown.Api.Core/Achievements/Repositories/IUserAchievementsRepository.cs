using AntiClown.Api.Core.Achievements.Domain;

namespace AntiClown.Api.Core.Achievements.Repositories;

public interface IUserAchievementsRepository
{
    Task<UserAchievement[]> GetByUserIdAsync(Guid userId);
    Task<UserAchievement[]> GetByAchievementIdAsync(Guid achievementId);
    Task CreateAsync(UserAchievement userAchievement);
}
