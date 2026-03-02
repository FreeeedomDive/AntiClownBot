using AntiClown.Api.Core.Achievements.Domain;
using AntiClown.Api.Core.Achievements.Services;
using AntiClown.Api.Dto.Achievements;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/achievements")]
public class AchievementsController(
    IAchievementsService achievementsService,
    IMapper mapper
) : Controller
{
    [HttpGet]
    public async Task<ActionResult<AchievementDto[]>> ReadAll()
    {
        var result = await achievementsService.ReadAllAsync();
        return mapper.Map<AchievementDto[]>(result);
    }

    [HttpGet("{achievementId:guid}")]
    public async Task<ActionResult<AchievementDto>> Read([FromRoute] Guid achievementId)
    {
        var result = await achievementsService.ReadAsync(achievementId);
        return mapper.Map<AchievementDto>(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] NewAchievementDto newAchievement)
    {
        var domainModel = mapper.Map<NewAchievement>(newAchievement);
        return await achievementsService.CreateAsync(domainModel);
    }

    [HttpDelete("{achievementId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid achievementId)
    {
        await achievementsService.DeleteAsync(achievementId);
        return NoContent();
    }

    [HttpGet("{achievementId:guid}/users")]
    public async Task<ActionResult<UserAchievementDto[]>> GetUsersByAchievement([FromRoute] Guid achievementId)
    {
        var result = await achievementsService.GetByAchievementIdAsync(achievementId);
        return mapper.Map<UserAchievementDto[]>(result);
    }

    [HttpPost("{achievementId:guid}/grant")]
    public async Task<ActionResult> Grant([FromRoute] Guid achievementId, [FromBody] GrantAchievementDto grant)
    {
        await achievementsService.GrantAsync(achievementId, grant.UserId, grant.GrantedAt);
        return NoContent();
    }

    [HttpGet("users/{userId:guid}")]
    public async Task<ActionResult<UserAchievementDto[]>> GetAchievementsByUser([FromRoute] Guid userId)
    {
        var result = await achievementsService.GetByUserIdAsync(userId);
        return mapper.Map<UserAchievementDto[]>(result);
    }
}
