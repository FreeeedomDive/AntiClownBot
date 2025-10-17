﻿using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Exceptions.F1Predictions;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Web.Api.Attributes;
using AntiClown.Web.Api.Dto;
using AntiClown.Web.Api.ExternalClients.F1FastApi;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/f1Predictions"), RequireUserToken]
public class F1PredictionsController(
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    IF1FastApiClient f1FastApiClient,
    ILogger<F1PredictionsController> logger
) : Controller
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

    [HttpGet("{raceId:guid}/raceResult")]
    public async Task<ActionResult<F1RaceResultDto>> GetRaceResult([FromRoute] Guid raceId)
    {
        try
        {
            var race = await antiClownEntertainmentApiClient.F1Predictions.ReadAsync(raceId);
            var result = await f1FastApiClient.GetF1PredictionRaceResult(raceId, race.IsSprint);
            return new F1RaceResultDto
            {
                Success = true,
                Result = result,
            };
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Exception when trying to get classifications from F1FastApi");
            return new F1RaceResultDto
            {
                Success = false,
                Result = null,
            };
        }
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
    public async Task<ActionResult<Dictionary<Guid, F1PredictionUserResultDto?[]>>> ReadStandingsAsync(int? season = null)
    {
        return await antiClownEntertainmentApiClient.F1Predictions.ReadStandingsAsync(season);
    }

    [HttpGet("charts")]
    public async Task<ActionResult<F1ChartsDto>> ReadCharts(int? season = null)
    {
        season ??= DateTime.Now.Year;
        var standings = await antiClownEntertainmentApiClient.F1Predictions.ReadStandingsAsync(season);
        var races = await antiClownEntertainmentApiClient.F1Predictions.FindAsync(
            new F1RaceFilterDto
            {
                Season = season.Value,
                IsActive = false,
            }
        );
        var charts = GetUsersCharts(standings);
        var result = new F1ChartsDto
        {
            UsersCharts = charts,
            ChampionChart = GetChampionChart(charts.FirstOrDefault(), races),
        };

        return result;
    }

    private static F1UserChartDto[] GetUsersCharts(Dictionary<Guid, F1PredictionUserResultDto?[]> standings)
    {
        var usersCharts = new List<F1UserChartDto>();
        foreach (var userResults in standings)
        {
            var totalPoints = 0;
            var userPoints = new List<int>([0]);
            foreach (var userResult in userResults.Value)
            {
                totalPoints += userResult?.TotalPoints ?? 0;
                userPoints.Add(totalPoints);
            }

            usersCharts.Add(
                new F1UserChartDto
                {
                    UserId = userResults.Key,
                    Points = userPoints.ToArray(),
                }
            );
        }

        return usersCharts.OrderByDescending(x => x.Points.Last()).ToArray();
    }

    private static F1UserChartDto GetChampionChart(
        F1UserChartDto? leaderChart,
        F1RaceDto[] races
    )
    {
        var result = new F1UserChartDto
        {
            UserId = Guid.Empty,
            Points = [],
        };
        var season = races.FirstOrDefault()?.Season;
        if (leaderChart is null || season is null)
        {
            return result;
        }

        var totalRaces = GetTotalRacesCount(season.Value);
        var championPoints = leaderChart
                             .Points
                             .Select((points, raceNumber) => raceNumber == 0
                                 ? 0
                                 : Math.Max(0, points - (totalRaces - raceNumber) * GetMaxPointsPerRace(races[raceNumber - 1]))
                             )
                             .ToArray();

        return result with { Points = championPoints };
    }

    private static int GetTotalRacesCount(int season)
    {
        return season switch
        {
            2023 => 20,
            2024 or 2025 => 30,
            _ => 0,
        };
    }

    private static int GetMaxPointsPerRace(F1RaceDto race)
    {
        return race.Season switch
        {
            2023 => 30,
            2024 => 55,
            2025 when race.IsSprint => F1PredictionsPointsHelper.CalculateSprintPoints(55),
            2025 when !race.IsSprint => 55,
            _ => 0,
        };
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