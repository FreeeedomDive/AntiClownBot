using AntiClown.Entertainment.Api.Core.CommonEvents.Services;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.GuessNumber;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers;

public class GuessNumberEventController : Controller
{
    public GuessNumberEventController(IGuessNumberEventService guessNumberEventService)
    {
        this.guessNumberEventService = guessNumberEventService;
    }

    private readonly IGuessNumberEventService guessNumberEventService;
}