using AntiClown.Entertainment.Api.Client;
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
            ChampionChart = GetChampionChart(season.Value, charts.FirstOrDefault(), races),
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

        var ordered = usersCharts
                      .OrderByDescending(x => x.Points.Last())
                      .ThenByDescending(x => standings[x.UserId].Count(p => p?.TenthPlacePoints == F1PredictionsPointsHelper.MaxPointsForTenthPlacePrediction))
                      .ToArray();

        return ordered;
    }

    private static F1UserChartDto GetChampionChart(
        int season,
        F1UserChartDto? leaderChart,
        F1RaceDto[] races
    )
    {
        var result = new F1UserChartDto
        {
            UserId = Guid.Empty,
            Points = [],
        };
        if (leaderChart is null)
        {
            return result;
        }

        var (racesCount, sprintsCount) = GetTotalRacesCount(season);
        var totalPointsLeft =
            racesCount * GetMaxPointsPerRace(season)
            + sprintsCount * F1PredictionsPointsHelper.CalculateSprintPoints(GetMaxPointsPerRace(season));
        var championPoints = leaderChart
                             .Points
                             .Select((points, raceNumber) =>
                                 {
                                     if (raceNumber == 0)
                                     {
                                         return Math.Max(0, points - totalPointsLeft);
                                     }

                                     var race = races[raceNumber - 1];
                                     totalPointsLeft -= race.IsSprint && sprintsCount > 0
                                         ? F1PredictionsPointsHelper.CalculateSprintPoints(GetMaxPointsPerRace(season))
                                         : GetMaxPointsPerRace(season);

                                     return Math.Max(0, points - totalPointsLeft);
                                 }
                             )
                             .ToArray();

        return result with { Points = championPoints };
    }

    private static (int RacesCount, int SprintsCount) GetTotalRacesCount(int season)
    {
        return season switch
        {
            2023 => (20, 0), /* sprints doesn't count */
            2024 => (30, 0), /* sprints give as many points as normal race */ 
            2025 => (24, 6), /* sprints worth 30% of normal race points */
            _ => (0, 0),
        };
    }

    private static int GetMaxPointsPerRace(int season)
    {
        return season switch
        {
            2023 => 30,
            2024 or 2025 => 55,
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