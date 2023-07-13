using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.ActiveEventsIndex;
using AntiClown.EntertainmentApi.Dto.CommonEvents;
using AntiClown.EntertainmentApi.Dto.CommonEvents.ActiveEventsIndex;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Common;

[Route("entertainmentApi/events/common/index")]
public class ActiveEventsIndexController : Controller
{
    public ActiveEventsIndexController(
        IActiveEventsIndexService activeEventsIndexService,
        IMapper mapper
    )
    {
        this.activeEventsIndexService = activeEventsIndexService;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<Dictionary<CommonEventTypeDto, bool>>> ReadAllEventTypes()
    {
        var result = await activeEventsIndexService.ReadAllEventTypesAsync();
        return mapper.Map<Dictionary<CommonEventTypeDto, bool>>(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<CommonEventTypeDto[]>> ReadActiveEventsAsync()
    {
        var result = await activeEventsIndexService.ReadActiveEventsAsync();
        return mapper.Map<CommonEventTypeDto[]>(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] ActiveCommonEventIndexDto activeCommonEventIndexDto)
    {
        await activeEventsIndexService.CreateAsync(mapper.Map<CommonEventType>(activeCommonEventIndexDto.EventType), activeCommonEventIndexDto.IsActive);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> Update([FromBody] ActiveCommonEventIndexDto activeCommonEventIndexDto)
    {
        await activeEventsIndexService.UpdateAsync(mapper.Map<CommonEventType>(activeCommonEventIndexDto.EventType), activeCommonEventIndexDto.IsActive);
        return NoContent();
    }

    [HttpPost("actualize")]
    public async Task<ActionResult> ActualizeIndex()
    {
        await activeEventsIndexService.ActualizeIndexAsync();
        return NoContent();
    }

    private readonly IActiveEventsIndexService activeEventsIndexService;
    private readonly IMapper mapper;
}