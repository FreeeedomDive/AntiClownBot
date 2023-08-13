using AntiClown.Entertainment.Api.Core.DailyEvents.Services.Announce;
using AntiClown.Entertainment.Api.Dto.DailyEvents.Announce;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Daily;

[Route("entertainmentApi/events/daily/announce")]
public class AnnounceEventController : Controller
{
    public AnnounceEventController(
        IAnnounceEventService announceEventService,
        IMapper mapper
    )
    {
        this.announceEventService = announceEventService;
        this.mapper = mapper;
    }

    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<AnnounceEventDto>> Read([FromRoute] Guid eventId)
    {
        var result = await announceEventService.ReadAsync(eventId);
        return mapper.Map<AnnounceEventDto>(result);
    }

    [HttpPost("start")]
    public async Task<ActionResult<Guid>> StartNew()
    {
        return await announceEventService.StartNewEventAsync();
    }

    private readonly IAnnounceEventService announceEventService;
    private readonly IMapper mapper;
}