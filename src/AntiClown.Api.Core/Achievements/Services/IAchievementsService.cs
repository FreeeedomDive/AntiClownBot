using AntiClown.Api.Core.Achievements.Domain;

namespace AntiClown.Api.Core.Achievements.Services;

public interface IAchievementsService
{
    Task<Achievement[]> ReadAllAsync();
    Task<Achievement> ReadAsync(Guid id);
    Task<Guid> CreateAsync(NewAchievement newAchievement);
    Task DeleteAsync(Guid id);

    Task<UserAchievement[]> GetByUserIdAsync(Guid userId);
    Task<UserAchievement[]> GetByAchievementIdAsync(Guid achievementId);
    Task GrantAsync(Guid achievementId, Guid userId, DateTime grantedAt);
}
