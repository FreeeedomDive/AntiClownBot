using AntiClown.Entertainment.Api.Core.DailyEvents.Domain;
using AntiClown.Entertainment.Api.Core.DailyEvents.Services.ActiveEventsIndex;
using AntiClown.EntertainmentApi.Dto.DailyEvents;
using AntiClown.EntertainmentApi.Dto.DailyEvents.ActiveEventsIndex;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Daily;

[Route("entertainmentApi/events/daily/index")]
public class ActiveDailyEventsIndexController : Controller
{
    public ActiveDailyEventsIndexController(
        IActiveDailyEventsIndexService activeDailyEventsIndexService,
        IMapper mapper
    )
    {
        this.activeDailyEventsIndexService = activeDailyEventsIndexService;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<Dictionary<DailyEventTypeDto, bool>>> ReadAllEventTypes()
    {
        var result = await activeDailyEventsIndexService.ReadAllEventTypesAsync();
        return mapper.Map<Dictionary<DailyEventTypeDto, bool>>(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<DailyEventTypeDto[]>> ReadActiveEventsAsync()
    {
        var result = await activeDailyEventsIndexService.ReadActiveEventsAsync();
        return mapper.Map<DailyEventTypeDto[]>(result);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] ActiveDailyEventIndexDto activeDailyEventIndexDto)
    {
        await activeDailyEventsIndexService.CreateAsync(mapper.Map<DailyEventType>(activeDailyEventIndexDto.EventType), activeDailyEventIndexDto.IsActive);
        return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult> Update([FromBody] ActiveDailyEventIndexDto activeDailyEventIndexDto)
    {
        await activeDailyEventsIndexService.UpdateAsync(mapper.Map<DailyEventType>(activeDailyEventIndexDto.EventType), activeDailyEventIndexDto.IsActive);
        return NoContent();
    }

    [HttpPost("actualize")]
    public async Task<ActionResult> ActualizeIndex()
    {
        await activeDailyEventsIndexService.ActualizeIndexAsync();
        return NoContent();
    }

    private readonly IActiveDailyEventsIndexService activeDailyEventsIndexService;
    private readonly IMapper mapper;
}