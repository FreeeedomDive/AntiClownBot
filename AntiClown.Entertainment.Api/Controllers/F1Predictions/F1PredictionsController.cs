using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.Entertainment.Api.Controllers.F1Predictions;

[Route("entertainmentApi/f1Predictions")]
public class F1PredictionsController : Controller
{
    public F1PredictionsController(
        IF1PredictionsService f1PredictionsService,
        IMapper mapper,
        ILoggerClient loggerClient
    )
    {
        this.f1PredictionsService = f1PredictionsService;
        this.mapper = mapper;
        this.loggerClient = loggerClient;
    }

    [HttpGet("active")]
    public async Task<ActionResult<F1RaceDto[]>> ReadActiveAsync()
    {
        var result = await f1PredictionsService.ReadActiveAsync();
        return mapper.Map<F1RaceDto[]>(result);
    }

    [HttpGet("{raceId:guid}")]
    public async Task<ActionResult<F1RaceDto>> ReadAsync(Guid raceId)
    {
        var result = await f1PredictionsService.ReadAsync(raceId);
        return mapper.Map<F1RaceDto>(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> StartNewRaceAsync([FromQuery] string name)
    {
        return await f1PredictionsService.StartNewRaceAsync(name);
    }

    [HttpPost("{raceId:guid}/addPrediction")]
    public async Task<ActionResult> AddPredictionAsync([FromRoute] Guid raceId, [FromBody] F1PredictionDto prediction)
    {
        var dtoJson = JsonConvert.SerializeObject(prediction, Formatting.Indented);
        var model = mapper.Map<F1Prediction>(prediction);
        var modelJson = JsonConvert.SerializeObject(prediction, Formatting.Indented);
        await loggerClient.InfoAsync("Json from front: {front}, json from businessModel: {businessModel}", dtoJson, modelJson);
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

    [HttpPost("{raceId:guid}/addClassification")]
    public async Task<ActionResult> AddClassificationsResult([FromRoute] Guid raceId, [FromBody] F1DriverDto[] f1Drivers)
    {
        await f1PredictionsService.AddClassificationsResultAsync(raceId, mapper.Map<F1Driver[]>(f1Drivers));
        return NoContent();
    }

    [HttpPost("{raceId:guid}/addDnf")]
    public async Task<ActionResult> AddDnfDriver([FromRoute] Guid raceId, [FromQuery] F1DriverDto dnfDriver)
    {
        await f1PredictionsService.AddDnfDriverAsync(raceId, mapper.Map<F1Driver>(dnfDriver));
        return NoContent();
    }

    [HttpPost("{raceId:guid}/addSafetyCar")]
    public async Task<ActionResult> AddSafetyCar([FromRoute] Guid raceId)
    {
        await f1PredictionsService.AddSafetyCarAsync(raceId);
        return NoContent();
    }

    [HttpPost("{raceId:guid}/addFirstPlaceLead")]
    public async Task<ActionResult> AddFirstPlaceLead([FromRoute] Guid raceId, [FromQuery] decimal firstPlaceLead)
    {
        await f1PredictionsService.AddFirstPlaceLeadAsync(raceId, firstPlaceLead);
        return NoContent();
    }

    [HttpPost("{raceId:guid}/finish")]
    public async Task<ActionResult<F1PredictionUserResultDto[]>> FinishRaceAsync([FromRoute] Guid raceId)
    {
        var result = await f1PredictionsService.FinishRaceAsync(raceId);
        return mapper.Map<F1PredictionUserResultDto[]>(result);
    }

    [HttpGet("standings")]
    public async Task<ActionResult<Dictionary<Guid, F1PredictionUserResultDto?[]>>> ReadStandingsAsync([FromQuery] int? season = null)
    {
        var result = await f1PredictionsService.ReadStandingsAsync(season);
        return mapper.Map<Dictionary<Guid, F1PredictionUserResultDto?[]>>(result);
    }

    private readonly IF1PredictionsService f1PredictionsService;
    private readonly IMapper mapper;
    private readonly ILoggerClient loggerClient;
}