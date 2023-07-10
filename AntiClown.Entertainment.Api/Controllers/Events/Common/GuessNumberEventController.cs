using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.GuessNumber;
using AntiClown.EntertainmentApi.Dto.CommonEvents.GuessNumber;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Common;

[Route("entertainmentApi/events/common/guessNumber")]
public class GuessNumberEventController : Controller
{
    public GuessNumberEventController(
        IGuessNumberEventService guessNumberEventService,
        IMapper mapper
    )
    {
        this.guessNumberEventService = guessNumberEventService;
        this.mapper = mapper;
    }

    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<GuessNumberEventDto>> Read([FromRoute] Guid eventId)
    {
        var result = await guessNumberEventService.ReadAsync(eventId);
        return mapper.Map<GuessNumberEventDto>(result);
    }

    [HttpPost("start")]
    public async Task<ActionResult<Guid>> StartNew()
    {
        return await guessNumberEventService.StartNewEventAsync();
    }

    [HttpPatch("{eventId:guid}/addPick")]
    public async Task<ActionResult> AddPick([FromRoute] Guid eventId, [FromBody] GuessNumberUserPickDto guessNumberUserPickDto)
    {
        await guessNumberEventService.AddParticipantAsync(eventId, guessNumberUserPickDto.UserId, mapper.Map<GuessNumberPick>(guessNumberUserPickDto.Pick));
        return NoContent();
    }

    [HttpPatch("{eventId:guid}/finish")]
    public async Task<ActionResult> Finish([FromRoute] Guid eventId)
    {
        await guessNumberEventService.FinishAsync(eventId);
        return NoContent();
    }

    private readonly IGuessNumberEventService guessNumberEventService;
    private readonly IMapper mapper;
}