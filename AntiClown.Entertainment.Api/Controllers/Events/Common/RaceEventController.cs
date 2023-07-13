using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Race;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Common;

[Route("entertainmentApi/events/common/race")]
public class RaceEventController : Controller
{
    public RaceEventController(
        IRaceService raceService,
        IMapper mapper
    )
    {
        this.raceService = raceService;
        this.mapper = mapper;
    }

    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<RaceEventDto>> Read([FromRoute] Guid eventId)
    {
        var result = await raceService.ReadAsync(eventId);
        return mapper.Map<RaceEventDto>(result);
    }

    [HttpPost("start")]
    public async Task<ActionResult<Guid>> StartNew()
    {
        return await raceService.StartNewEventAsync();
    }

    [HttpPost("{eventId:guid}/addParticipant")]
    public async Task<ActionResult> AddParticipant([FromRoute] Guid eventId, [FromQuery] Guid userId)
    {
        await raceService.AddParticipantAsync(eventId, userId);
        return NoContent();
    }

    [HttpPost("{eventId:guid}/finish")]
    public async Task<ActionResult> Finish([FromRoute] Guid eventId)
    {
        await raceService.FinishAsync(eventId);
        return NoContent();
    }

    private readonly IMapper mapper;

    private readonly IRaceService raceService;
}