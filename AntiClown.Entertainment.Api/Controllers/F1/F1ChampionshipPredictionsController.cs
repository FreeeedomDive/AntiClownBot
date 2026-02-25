using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


namespace AntiClown.Entertainment.Api.Controllers.F1;

[Route("entertainmentApi/f1ChampionshipPredictions")]
public class F1ChampionshipPredictionsController(
    IF1ChampionshipPredictionsService f1ChampionshipPredictionsService,
    IMapper mapper
) : Controller
{
    [HttpGet]
    public async Task<ActionResult<F1ChampionshipPredictionDto>> Read([FromQuery] Guid userId, [FromQuery] int season)
    {
        var result = await f1ChampionshipPredictionsService.ReadAsync(userId, season);
        return mapper.Map<F1ChampionshipPredictionDto>(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrUpdate([FromBody] F1ChampionshipPredictionDto dto)
    {
        var prediction = mapper.Map<F1ChampionshipPrediction>(dto);
        await f1ChampionshipPredictionsService.CreateOrUpdateAsync(prediction);
        return NoContent();
    }
}
