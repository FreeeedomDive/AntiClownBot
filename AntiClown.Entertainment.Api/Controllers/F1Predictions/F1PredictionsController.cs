using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
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
        await f1PredictionsService.AddPredictionAsync(
            raceId, prediction.UserId, mapper.Map<F1Driver>(prediction.TenthPlacePickedDriver), mapper.Map<F1Driver>(prediction.FirstDnfPickedDriver)
        );
        return NoContent();
    }

    [HttpPost("{raceId:guid}/close")]
    public async Task<ActionResult> ClosePredictionsAsync([FromRoute] Guid raceId)
    {
        await f1PredictionsService.ClosePredictionsAsync(raceId);
        return NoContent();
    }

    [HttpPost("{raceId:guid}/addFirstDnf")]
    public async Task<ActionResult> AddFirstDnfResultAsync([FromRoute] Guid raceId, [FromQuery] F1DriverDto firstDnfDriver)
    {
        await f1PredictionsService.AddFirstDnfResultAsync(raceId, mapper.Map<F1Driver>(firstDnfDriver));
        return NoContent();
    }

    [HttpPost("{raceId:guid}/addClassification")]
    public async Task<ActionResult> AddClassificationsResultAsync([FromRoute] Guid raceId, [FromBody] F1DriverDto[] f1Drivers)
    {
        await f1PredictionsService.AddClassificationsResultAsync(raceId, mapper.Map<F1Driver[]>(f1Drivers));
        return NoContent();
    }

    [HttpPost("{raceId:guid}/finish")]
    public async Task<ActionResult<F1PredictionResultDto[]>> FinishRaceAsync([FromRoute] Guid raceId)
    {
        var result = await f1PredictionsService.FinishRaceAsync(raceId);
        return mapper.Map<F1PredictionResultDto[]>(result);
    }

    [HttpGet("standings")]
    public async Task<ActionResult<Dictionary<Guid, F1PredictionResultDto?[]>>> ReadStandingsAsync()
    {
        var result = await f1PredictionsService.ReadStandingsAsync();
        return mapper.Map<Dictionary<Guid, F1PredictionResultDto?[]>>(result);
    }

    private readonly IF1PredictionsService f1PredictionsService;
    private readonly IMapper mapper;
}