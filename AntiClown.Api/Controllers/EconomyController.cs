using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Dto.Economies;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/economy")]
public class EconomyController : Controller
{
    public EconomyController(
        IEconomyService economyService,
        IMapper mapper
    )
    {
        this.economyService = economyService;
        this.mapper = mapper;
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<EconomyDto>> Read([FromRoute] Guid userId)
    {
        var economy = await economyService.ReadEconomyAsync(userId);
        return mapper.Map<EconomyDto>(economy);
    }

    [HttpPatch("{userId:guid}/scamCoins")]
    public async Task<ActionResult> UpdateScamCoins([FromRoute] Guid userId, [FromBody] UpdateScamCoinsDto updateScamCoinsDto)
    {
        await economyService.UpdateScamCoinsAsync(updateScamCoinsDto.UserId, updateScamCoinsDto.ScamCoinsDiff, updateScamCoinsDto.Reason);
        return NoContent();
    }

    [HttpPatch("{userId:guid}/lootBoxes")]
    public async Task<ActionResult> UpdateLootBoxes([FromRoute] Guid userId, [FromBody] UpdateLootBoxesDto lootBoxesDto)
    {
        await economyService.UpdateLootBoxesAsync(lootBoxesDto.UserId, lootBoxesDto.LootBoxesDiff);
        return NoContent();
    }

    [HttpPatch("resetAllCoolDowns")]
    public async Task<ActionResult> ResetAllCoolDowns()
    {
        await economyService.ResetAllCoolDownsAsync();
        return NoContent();
    }

    private readonly IEconomyService economyService;
    private readonly IMapper mapper;
}