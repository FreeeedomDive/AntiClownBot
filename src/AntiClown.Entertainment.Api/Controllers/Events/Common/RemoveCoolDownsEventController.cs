using AntiClown.Entertainment.Api.Core.CommonEvents.Services.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Common;

[Route("entertainmentApi/events/common/removeCoolDowns")]
public class RemoveCoolDownsEventController : Controller
{
    public RemoveCoolDownsEventController(
        IRemoveCoolDownsEventService removeCoolDownsEventService,
        IMapper mapper
    )
    {
        this.removeCoolDownsEventService = removeCoolDownsEventService;
        this.mapper = mapper;
    }

    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<RemoveCoolDownsEventDto>> Read([FromRoute] Guid eventId)
    {
        var result = await removeCoolDownsEventService.ReadAsync(eventId);
        return mapper.Map<RemoveCoolDownsEventDto>(result);
    }

    [HttpPost("start")]
    public async Task<ActionResult<Guid>> StartNew()
    {
        return await removeCoolDownsEventService.StartNewEventAsync();
    }

    private readonly IMapper mapper;

    private readonly IRemoveCoolDownsEventService removeCoolDownsEventService;
}