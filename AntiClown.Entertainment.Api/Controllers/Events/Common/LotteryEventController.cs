using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Lottery;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Lottery;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Common;

[Route("entertainmentApi/events/common/lottery")]
public class LotteryEventController : Controller
{
    public LotteryEventController(
        ILotteryService lotteryService,
        IMapper mapper
    )
    {
        this.lotteryService = lotteryService;
        this.mapper = mapper;
    }

    [HttpGet("{eventId:guid}")]
    public async Task<ActionResult<LotteryEventDto>> Read([FromRoute] Guid eventId)
    {
        var result = await lotteryService.ReadAsync(eventId);
        return mapper.Map<LotteryEventDto>(result);
    }

    [HttpPost("start")]
    public async Task<ActionResult<Guid>> StartNew()
    {
        return await lotteryService.StartNewEventAsync();
    }

    [HttpPost("{eventId:guid}/addParticipant")]
    public async Task<ActionResult> AddParticipant([FromRoute] Guid eventId, [FromQuery] Guid userId)
    {
        await lotteryService.AddParticipantAsync(eventId, userId);
        return NoContent();
    }

    [HttpPost("{eventId:guid}/finish")]
    public async Task<ActionResult> Finish([FromRoute] Guid eventId)
    {
        await lotteryService.FinishAsync(eventId);
        return NoContent();
    }

    private readonly ILotteryService lotteryService;
    private readonly IMapper mapper;
}