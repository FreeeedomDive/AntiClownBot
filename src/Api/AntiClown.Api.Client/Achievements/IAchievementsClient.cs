using AntiClown.Api.Dto.Achievements;

namespace AntiClown.Api.Client.Achievements;

public interface IAchievementsClient
{
    Task<AchievementDto[]> ReadAllAsync();
    Task<AchievementDto> ReadAsync(Guid achievementId);
    Task<Guid> CreateAsync(NewAchievementDto newAchievement);
    Task DeleteAsync(Guid achievementId);
    Task<UserAchievementDto[]> GetUsersByAchievementAsync(Guid achievementId);
    Task GrantAsync(Guid achievementId, GrantAchievementDto grant);
    Task<UserAchievementDto[]> GetAchievementsByUserAsync(Guid userId);
}
