﻿using AntiClown.Entertainment.Api.Core.F1Predictions.Services.Statistics;
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

    [HttpGet("mostPickedDrivers")]
    public async Task<MostPickedDriversStatsDto> GetMostPickedDrivers()
    {
        var result = await f1PredictionsStatisticsService.GetMostPickedDriversAsync();
        return mapper.Map<MostPickedDriversStatsDto>(result);
    }

    [HttpGet("{userId:guid}/mostPickedDrivers")]
    public async Task<MostPickedDriversStatsDto> GetMostPickedDrivers([FromRoute] Guid userId)
    {
        var result = await f1PredictionsStatisticsService.GetMostPickedDriversAsync(userId);
        return mapper.Map<MostPickedDriversStatsDto>(result);
    }

    [HttpGet("mostProfitableDrivers")]
    public async Task<MostProfitableDriversStatsDto> GetMostProfitableDrivers()
    {
        var result = await f1PredictionsStatisticsService.GetMostProfitableDriversAsync();
        return mapper.Map<MostProfitableDriversStatsDto>(result);
    }

    [HttpGet("{userId:guid}/userPointsStats")]
    public async Task<UserPointsStatsDto> GetUserPointsStats([FromRoute] Guid userId)
    {
        var result = await f1PredictionsStatisticsService.GetUserPointsStatsAsync(userId);
        return mapper.Map<UserPointsStatsDto>(result);
    }

    private readonly IF1PredictionsStatisticsService f1PredictionsStatisticsService;
    private readonly IMapper mapper;
}