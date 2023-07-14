using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Transfusion;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Common;

[Route("entertainmentApi/events/common/transfusion")]
public class TransfusionEventController : Controller
{
    public TransfusionEventController(
        ITransfusionEventService transfusionEventService,
        IMapper mapper
    )
    {
        this.transfusionEventService = transfusionEventService;
        this.mapper = mapper;
    }

    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<TransfusionEventDto>> Read([FromRoute] Guid eventId)
    {
        var result = await transfusionEventService.ReadAsync(eventId);
        return mapper.Map<TransfusionEventDto>(result);
    }

    [HttpPost("start")]
    public async Task<ActionResult<Guid>> StartNew()
    {
        return await transfusionEventService.StartNewEventAsync();
    }

    private readonly IMapper mapper;

    private readonly ITransfusionEventService transfusionEventService;
}