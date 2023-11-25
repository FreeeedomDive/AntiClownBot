using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Entertainment.Api.Controllers.F1Predictions;

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

    [HttpGet("mostPickedDriversByUsers")]
    public async Task<MostPickedDriversByUsersStatsDto> GetMostPickedDriversByUsersAsync()
    {
        var result = await f1PredictionsStatisticsService.GetMostPickedDriversByUsersAsync();
        return mapper.Map<MostPickedDriversByUsersStatsDto>(result);
    }

    [HttpGet("mostProfitableDrivers")]
    public async Task<MostProfitableDriversStatsDto> GetMostProfitableDriversAsync()
    {
        var result = await f1PredictionsStatisticsService.GetMostProfitableDriversAsync();
        return mapper.Map<MostProfitableDriversStatsDto>(result);
    }

    private readonly IF1PredictionsStatisticsService f1PredictionsStatisticsService;
    private readonly IMapper mapper;
}