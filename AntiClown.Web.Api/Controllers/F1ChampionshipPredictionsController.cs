using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions;
using AntiClown.Web.Api.Attributes;
using AntiClown.Web.Api.Dto.F1Predictions;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/f1ChampionshipPredictions"), RequireUserToken]
public class F1ChampionshipPredictionsController(
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient
) : Controller
{
    [HttpGet]
    public async Task<ActionResult<F1ChampionshipPredictionDto>> Read([FromQuery] Guid userId, [FromQuery] int season)
    {
        return await antiClownEntertainmentApiClient.F1ChampionshipPredictions.ReadAsync(userId, season);
    }

    [HttpPost]
    public async Task<ActionResult<AddPredictionResultDto>> CreateOrUpdate([FromBody] F1ChampionshipPredictionDto dto)
    {
        try
        {
            await antiClownEntertainmentApiClient.F1ChampionshipPredictions.CreateOrUpdateAsync(dto);
            return AddPredictionResultDto.Success;
        }
        catch (ChampionshipPredictionsClosedException)
        {
            return AddPredictionResultDto.PredictionsClosed;
        }
    }

    [HttpGet("results")]
    public async Task<ActionResult<F1ChampionshipResultsDto>> ReadResults([FromQuery] int season)
    {
        return await antiClownEntertainmentApiClient.F1ChampionshipPredictions.ReadResultsAsync(season);
    }

    [HttpPost("results")]
    public async Task<ActionResult> WriteResults([FromQuery] int season, [FromBody] F1ChampionshipResultsDto dto)
    {
        await antiClownEntertainmentApiClient.F1ChampionshipPredictions.WriteResultsAsync(season, dto);
        return NoContent();
    }

    [HttpGet("points")]
    public async Task<ActionResult<F1ChampionshipUserPointsDto[]>> BuildPoints([FromQuery] int season)
    {
        return await antiClownEntertainmentApiClient.F1ChampionshipPredictions.BuildPointsAsync(season);
    }
}
