using AntiClown.F1Predictions.Client;
using AntiClown.F1Predictions.Dto;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[ApiController]
[Route("webApi/v2/f1Predictions/championship/{season:int}"), RequireUserToken]
public class F1ChampionshipPredictionsV2Controller(IF1PredictionsClient predictions) : Controller
{
    [HttpGet]
    public Task<ActionResult<ChampionshipDetailsDto>> GetChampionship(int season, CancellationToken cancellationToken) =>
        ProxyAsync(() => predictions.GetChampionshipAsync(ParticipantId, season, cancellationToken));

    [HttpPut("predictions/{stage}")]
    public async Task<ActionResult<ChampionshipPredictionDto>> SaveChampionshipPrediction(int season, ChampionshipStageDto stage,
        [FromBody] SaveChampionshipPredictionRequestDto request, CancellationToken cancellationToken)
    {
        return await ProxyAsync(() =>
            predictions.SaveChampionshipPredictionAsync(ParticipantId, season, stage, request, cancellationToken));
    }

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
