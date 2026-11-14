using AntiClown.F1Predictions.Client;
using AntiClown.F1Predictions.Dto;
using AntiClown.Web.Api.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/v2/f1Predictions"), RequireUserToken]
public class F1PredictionsV2Controller(IF1PredictionsClient predictions) : Controller
{
    [HttpGet("races")]
    public Task<ActionResult<ListResponseDto<RaceDto>>> GetRaces([FromQuery] int season, CancellationToken cancellationToken) =>
        ProxyAsync(async () => new ListResponseDto<RaceDto>(await predictions.GetRacesAsync(ParticipantId, season, cancellationToken)));

    [HttpGet("races/{raceId:guid}")]
    public Task<ActionResult<UserRaceDto>> GetRace(Guid raceId, CancellationToken cancellationToken) =>
        ProxyAsync(() => predictions.GetRaceAsync(ParticipantId, raceId, cancellationToken));

    [HttpPut("races/{raceId:guid}/prediction")]
    public Task<ActionResult<PredictionDto>> SavePrediction(Guid raceId, [FromBody] SavePredictionRequestDto request,
        CancellationToken cancellationToken) =>
        ProxyAsync(() => predictions.SavePredictionAsync(ParticipantId, raceId, request, cancellationToken));

    [HttpGet("standings")]
    public Task<ActionResult<SeasonStandingsDto>> GetStandings([FromQuery] int season, CancellationToken cancellationToken) =>
        ProxyAsync(() => predictions.GetStandingsAsync(season, cancellationToken));

    [HttpGet("stats")]
    public Task<ActionResult<SeasonStatsDto>> GetStats([FromQuery] int season, CancellationToken cancellationToken) =>
        ProxyAsync(() => predictions.GetStatsAsync(ParticipantId, season, cancellationToken));

    [HttpGet("teams")]
    public Task<ActionResult<ListResponseDto<TeamDto>>> GetTeams([FromQuery] int? season, CancellationToken cancellationToken) =>
        ProxyAsync(async () => new ListResponseDto<TeamDto>(await predictions.GetTeamsAsync(ParticipantId, season ?? DateTime.UtcNow.Year, cancellationToken)));

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
