using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Statistics;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.F1;

[Route("entertainmentApi/f1Predictions/stats")]
public class F1PredictionsStatsController : Controller
{
    public F1PredictionsStatsController(
        IF1PredictionsStatisticsService f1PredictionsStatisticsService,
        IMapper mapper
    )
    {
        this.f1PredictionsStatisticsService = f1PredictionsStatisticsService;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<F1SeasonStatsDto> GetSeasonStats([FromQuery] int season)
    {
        var result = await f1PredictionsStatisticsService.GetSeasonStatsAsync(season);
        return mapper.Map<F1SeasonStatsDto>(result);
    }

    private readonly IF1PredictionsStatisticsService f1PredictionsStatisticsService;
    private readonly IMapper mapper;
}
