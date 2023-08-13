using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Dto.Economies;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/economy")]
public class LohotronController : Controller
{
    public LohotronController(
        ILohotronService lohotronService,
        IMapper mapper
    )
    {
        this.lohotronService = lohotronService;
        this.mapper = mapper;
    }

    [HttpPost("{userId:guid}/lohotron")]
    public async Task<ActionResult<LohotronRewardDto>> UseLohotron([FromRoute] Guid userId)
    {
        var result = await lohotronService.UseLohotronAsync(userId);
        return mapper.Map<LohotronRewardDto>(result);
    }

    [HttpPost("lohotron/reset")]
    public async Task<ActionResult> Reset()
    {
        await lohotronService.ResetLohotronForAllAsync();
        return NoContent();
    }

    private readonly ILohotronService lohotronService;
    private readonly IMapper mapper;
}