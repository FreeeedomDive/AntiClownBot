namespace AntiClown.Entertainment.Api.Dto.F1Predictions;

public static class F1PredictionsHelper
{
    public static (int RacesCount, int SprintsCount) GetTotalRacesCount(int season)
    {
        return season switch
        {
            2023 => (20, 0), /* sprints doesn't count */
            2024 => (30, 0), /* sprints give as many points as normal race */
            2025 => (24, 6), /* sprints worth 30% of normal race points */
            2026 => (24, 0), /* sprints doesn't count */
            _ => (0, 0),
        };
    }

    public static int GetMaxPointsPerRace(int season)
    {
        return season switch
        {
            2023 => 30,
            2024 or 2025 or 2026 => 55,
            _ => 0,
        };
    }

    public static int CalculatePoints(int points, int season, bool isSprint)
    {
        return season switch
        {
            2025 when isSprint => points * SprintRacePointsPercent / 100,
            _ => points,
        };
    }

    public static int GetPositionPredictionPoints(int predictedPosition, int actualPosition)
    {
        var diff = Math.Abs(actualPosition - predictedPosition);
        return diff switch
        {
            0 => 10,
            1 => 7,
            2 => 4,
            3 => 1,
            _ => 0,
        };
    }

    private const int SprintRacePointsPercent = 30;

    public const int NoDnfPredictionPoints = 10;
    public const int DnfPredictionPoints = 2;

    public const int IncidentsPredictionPoints = 5;

    public const int MaxPointsForTenthPlacePrediction = 25;

    public static readonly Dictionary<int, int> PointsByFinishPlaceDistribution = new()
    {
        { 1, 1 },
        { 2, 2 },
        { 3, 4 },
        { 4, 6 },
        { 5, 8 },
        { 6, 10 },
        { 7, 12 },
        { 8, 15 },
        { 9, 18 },
        { 10, MaxPointsForTenthPlacePrediction },
        { 11, 18 },
        { 12, 15 },
        { 13, 12 },
        { 14, 10 },
        { 15, 8 },
        { 16, 6 },
        { 17, 4 },
        { 18, 2 },
        { 19, 1 },
        { 20, 1 },
        { 21, 0 },
        { 22, 0 },
    };
}