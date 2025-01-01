using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.F1Predictions;

[Route("entertainmentApi/f1Predictions")]
public class F1PredictionsController : Controller
{
    public F1PredictionsController(
        IF1PredictionsService f1PredictionsService,
        IMapper mapper
    )
    {
        this.f1PredictionsService = f1PredictionsService;
        this.mapper = mapper;
    }

    [HttpPost("find")]
    public async Task<ActionResult<F1RaceDto[]>> Find([FromBody] F1RaceFilterDto filter)
    {
        var result = await f1PredictionsService.FindAsync(mapper.Map<F1RaceFilter>(filter));
        return mapper.Map<F1RaceDto[]>(result);
    }

    [HttpGet("{raceId:guid}")]
    public async Task<ActionResult<F1RaceDto>> Read([FromRoute] Guid raceId)
    {
        var result = await f1PredictionsService.ReadAsync(raceId);
        return mapper.Map<F1RaceDto>(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> StartNewRace([FromQuery] string name, [FromQuery] bool isSprint)
    {
        return await f1PredictionsService.StartNewRaceAsync(name, isSprint);
    }

    [HttpPost("{raceId:guid}/addPrediction")]
    public async Task<ActionResult> AddPrediction([FromRoute] Guid raceId, [FromBody] F1PredictionDto prediction)
    {
        var model = mapper.Map<F1Prediction>(prediction);
        await f1PredictionsService.AddPredictionAsync(raceId, prediction.UserId, model);
        return NoContent();
    }

    [HttpPost("{raceId:guid}/close")]
    public async Task<ActionResult> ClosePredictions([FromRoute] Guid raceId)
    {
        await f1PredictionsService.ClosePredictionsAsync(raceId);
        return NoContent();
    }

    [HttpPost("{raceId:guid}/addResult")]
    public async Task<ActionResult> AddResult([FromRoute] Guid raceId, [FromBody] F1PredictionRaceResultDto raceResult)
    {
        await f1PredictionsService.AddRaceResultAsync(raceId, mapper.Map<F1PredictionRaceResult>(raceResult));
        return NoContent();
    }

    [HttpPost("{raceId:guid}/finish")]
    public async Task<ActionResult<F1PredictionUserResultDto[]>> FinishRace([FromRoute] Guid raceId)
    {
        var result = await f1PredictionsService.FinishRaceAsync(raceId);
        return mapper.Map<F1PredictionUserResultDto[]>(result);
    }

    [HttpGet("{raceId:guid}/results")]
    public async Task<ActionResult<F1PredictionUserResultDto[]>> ReadResults([FromRoute] Guid raceId)
    {
        var result = await f1PredictionsService.ReadRaceResultsAsync(raceId);
        return mapper.Map<F1PredictionUserResultDto[]>(result);
    }

    [HttpGet("standings")]
    public async Task<ActionResult<Dictionary<Guid, F1PredictionUserResultDto?[]>>> ReadStandings([FromQuery] int? season = null)
    {
        var result = await f1PredictionsService.ReadStandingsAsync(season);
        return mapper.Map<Dictionary<Guid, F1PredictionUserResultDto?[]>>(result);
    }

    [HttpGet("teams")]
    public async Task<ActionResult<F1TeamDto[]>> ReadTeams()
    {
        var result = await f1PredictionsService.GetActiveTeamsAsync();
        return mapper.Map<F1TeamDto[]>(result);
    }

    [HttpPost("teams")]
    public async Task<ActionResult> CreateOrUpdateTeam([FromBody] F1TeamDto dto)
    {
        var team = mapper.Map<F1Team>(dto);
        await f1PredictionsService.CreateOrUpdateTeamAsync(team);

        return NoContent();
    }

    private readonly IF1PredictionsService f1PredictionsService;
    private readonly IMapper mapper;
}