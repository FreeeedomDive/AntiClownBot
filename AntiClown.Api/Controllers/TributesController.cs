using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Dto.Economies;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/economy/{userId:guid}/tributes")]
public class TributesController : Controller
{
    public TributesController(
        ITributeService tributeService,
        IMapper mapper
    )
    {
        this.tributeService = tributeService;
        this.mapper = mapper;
    }

    [HttpGet("when")]
    public async Task<ActionResult<NextTributeDto>> WhenNextTribute([FromRoute] Guid userId)
    {
        var result = await tributeService.WhenNextTributeAsync(userId);
        return new NextTributeDto
        {
            NextTributeDateTime = result
        };
    }

    [HttpPost]
    public async Task<ActionResult<TributeDto>> Tribute(Guid userId)
    {
        var tribute = await tributeService.MakeTributeAsync(userId);
        return mapper.Map<TributeDto>(tribute);
    }

    private readonly ITributeService tributeService;
    private readonly IMapper mapper;
}