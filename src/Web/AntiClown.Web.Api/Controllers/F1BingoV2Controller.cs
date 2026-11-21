using AntiClown.F1Predictions.Client;
using AntiClown.F1Predictions.Dto;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[ApiController]
[Route("webApi/v2/f1Predictions/bingo/boards/me"), RequireUserToken]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class F1BingoV2Controller(IF1PredictionsClient predictions) : Controller
{
    [HttpGet]
    public Task<ActionResult<BingoDetailsDto>> GetOwnBingo([FromQuery] int season, CancellationToken cancellationToken) =>
        ProxyAsync(() => predictions.GetOwnBingoAsync(ParticipantId, season, cancellationToken));

    [HttpPut]
    public Task<ActionResult<BingoDetailsDto>> CreateOwnBingo([FromQuery] int season, CancellationToken cancellationToken) =>
        ProxyAsync(() => predictions.CreateOwnBingoAsync(ParticipantId, season, cancellationToken));

    private async Task<ActionResult<T>> ProxyAsync<T>(Func<Task<T>> request)
    {
        try
        {
            return await request();
        }
        catch (F1PredictionsException exception)
        {
            var status = exception.StatusCode is 401 or 403 ? 502 : exception.StatusCode;
            var problem = new ProblemDetails
            {
                Status = status,
                Title = exception.Title,
                Detail = exception.Message,
                Extensions =
                {
                    ["upstreamStatus"] = exception.StatusCode
                }
            };
            return StatusCode(status, problem);
        }
    }

    private Guid ParticipantId => Guid.Parse(Request.Cookies["userId"]!);
}
