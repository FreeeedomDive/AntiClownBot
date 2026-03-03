using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Bedge;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Common;

[Route("entertainmentApi/events/common/bedge")]
public class BedgeEventController : Controller
{
    public BedgeEventController(IBedgeService bedgeService)
    {
        this.bedgeService = bedgeService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<Guid>> StartNew()
    {
        return await bedgeService.StartNewEventAsync();
    }

    private readonly IBedgeService bedgeService;
}