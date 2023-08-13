using AntiClown.Entertainment.Api.Core.DailyEvents.Services.PaymentsAndResets;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.Events.Daily;

[Route("entertainmentApi/events/daily/paymentsAndResets")]
public class PaymentsAndResetsEventController : Controller
{
    public PaymentsAndResetsEventController(IPaymentsAndResetsService paymentsAndResetsService)
    {
        this.paymentsAndResetsService = paymentsAndResetsService;
    }

    [HttpPost("start")]
    public async Task<ActionResult<Guid>> StartNew()
    {
        return await paymentsAndResetsService.StartNewEventAsync();
    }

    private readonly IPaymentsAndResetsService paymentsAndResetsService;
}