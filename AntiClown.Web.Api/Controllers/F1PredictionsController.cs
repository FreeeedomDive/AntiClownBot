using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Web.Api.Attributes;
using AntiClown.Web.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/f1Predictions")]
[RequireUserToken]
public class F1PredictionsController(IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient) : Controller
{
    [HttpPost("find")]
    public async Task<ActionResult<F1RaceDto[]>> Find([FromBody] F1RaceFilterDto filter)
    {
        return await antiClownEntertainmentApiClient.F1Predictions.FindAsync(filter);
    }

    [HttpGet("{raceId:guid}")]
    public async Task<ActionResult<F1RaceDto>> Read([FromRoute] Guid raceId)
    {
        return await antiClownEntertainmentApiClient.F1Predictions.ReadAsync(raceId);
    }

    [HttpPost("{raceId:guid}/addPrediction")]
    public async Task<ActionResult<AddPredictionResultDto>> AddPrediction([FromRoute] Guid raceId, [FromBody] F1PredictionDto prediction)
    {
        try
        {
            await antiClownEntertainmentApiClient.F1Predictions.AddPredictionAsync(raceId, prediction);
            return AddPredictionResultDto.Success;
        }
        catch (PredictionsAlreadyClosedException)
        {
            return AddPredictionResultDto.PredictionsClosed;
        }
    }

    [HttpPost("{raceId:guid}/close")]
    public async Task<ActionResult> Close([FromRoute] Guid raceId)
    {
        await antiClownEntertainmentApiClient.F1Predictions.ClosePredictionsAsync(raceId);
        return NoContent();
    }

    [HttpPost("{raceId:guid}/addResult")]
    public async Task<ActionResult> AddResult([FromRoute] Guid raceId, [FromBody] F1PredictionRaceResultDto result)
    {
        await antiClownEntertainmentApiClient.F1Predictions.AddResultAsync(raceId, result);
        return NoContent();
    }

    [HttpPost("{raceId:guid}/finish")]
    public async Task<ActionResult> Finish([FromRoute] Guid raceId)
    {
        await antiClownEntertainmentApiClient.F1Predictions.FinishRaceAsync(raceId);
        return NoContent();
    }

    [HttpGet("standings")]
    public async Task<Dictionary<Guid, F1PredictionUserResultDto?[]>> ReadStandingsAsync(int? season = null)
    {
        return await antiClownEntertainmentApiClient.F1Predictions.ReadStandingsAsync(season);
    }

    [HttpGet("teams")]
    public async Task<ActionResult<F1TeamDto[]>> ReadTeams()
    {
        return await antiClownEntertainmentApiClient.F1Predictions.ReadTeamsAsync();
    }

    [HttpPost("teams")]
    public async Task<ActionResult> CreateOrUpdateTeam([FromBody] F1TeamDto dto)
    {
        await antiClownEntertainmentApiClient.F1Predictions.CreateOrUpdateTeamAsync(dto);
        return NoContent();
    }
}