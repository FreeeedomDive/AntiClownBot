using AntiClown.Api.Client;
using AntiClown.Web.Api.Attributes;
using AntiClown.Web.Api.Dto.Achievements;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/users/{userId}/achievements")]
[RequireUserToken]
public class AchievementsController(IAntiClownApiClient antiClownApiClient) : Controller
{
    [HttpGet]
    public async Task<UserAchievementWithDetailsDto[]> GetByUser([FromRoute] Guid userId)
    {
        var userAchievements = await antiClownApiClient.Achievements.GetAchievementsByUserAsync(userId);
        if (userAchievements.Length == 0)
        {
            return [];
        }

        var allAchievements = await antiClownApiClient.Achievements.ReadAllAsync();
        var achievementMap = allAchievements.ToDictionary(a => a.Id);

        return userAchievements
            .Where(ua => achievementMap.ContainsKey(ua.AchievementId))
            .Select(
                ua => new UserAchievementWithDetailsDto
                {
                    Id = ua.Id,
                    AchievementId = ua.AchievementId,
                    Name = achievementMap[ua.AchievementId].Name,
                    Logo = achievementMap[ua.AchievementId].Logo,
                    GrantedAt = ua.GrantedAt,
                }
            )
            .ToArray();
    }
}
