namespace AntiClown.EntertainmentApi.Dto.CommonEvents.Race;

public static class RaceHelper
{
    public static readonly Dictionary<int, int> PositionToPoints = new()
    {
        { 1, 25 },
        { 2, 18 },
        { 3, 15 },
        { 4, 12 },
        { 5, 10 },
        { 6, 8 },
        { 7, 6 },
        { 8, 4 },
        { 9, 2 },
        { 10, 1 },
    };

    public const int PointsToScamCoinsMultiplier = 100;
}