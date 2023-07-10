using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Lottery;
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

    private readonly ILotteryService lotteryService;
    private readonly IMapper mapper;
}