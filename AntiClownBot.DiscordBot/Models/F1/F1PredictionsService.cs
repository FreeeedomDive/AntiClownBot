using AntiClownDiscordBotVersion2.Utils.Extensions;

namespace AntiClownDiscordBotVersion2.Models.F1;

public class F1PredictionsService : IF1PredictionsService
{
    public F1PredictionsService()
    {
        tenthPlaceUserToDriverIndex = new Dictionary<ulong, F1Driver>();
        firstDnfUserToDriverIndex = new Dictionary<ulong, F1Driver>();
        tenthPlaceDriverToUsersIndex = new Dictionary<F1Driver, List<ulong>>();
        firstDnfDriverToUsersIndex = new Dictionary<F1Driver, List<ulong>>();
        raceResults = new List<F1Driver>();
    }

    public void PredictTenthPlace(ulong userId, F1Driver f1Driver)
    {
        var hasCurrentPrediction = tenthPlaceUserToDriverIndex.TryGetValue(userId, out var currentPrediction);
        if (hasCurrentPrediction)
        {
            tenthPlaceDriverToUsersIndex.Remove(currentPrediction, userId);
        }
        tenthPlaceUserToDriverIndex[userId] = f1Driver;
        tenthPlaceDriverToUsersIndex.Add(f1Driver, userId);
    }

    public void PredictDnf(ulong userId, F1Driver f1Driver)
    {
        var hasCurrentPrediction = firstDnfUserToDriverIndex.TryGetValue(userId, out var currentPrediction);
        if (hasCurrentPrediction)
        {
            firstDnfDriverToUsersIndex.Remove(currentPrediction, userId);
        }
        firstDnfUserToDriverIndex[userId] = f1Driver;
        firstDnfDriverToUsersIndex.Add(f1Driver, userId);
    }
    
    public Dictionary<ulong, F1Driver> GetTenthPlacePredictions()
    {
        return tenthPlaceUserToDriverIndex;
    }
    
    public Dictionary<ulong, F1Driver> GetFirstDnfPredictions()
    {
        return firstDnfUserToDriverIndex;
    }

    public void AddDriverToResult(F1Driver driver)
    {
        raceResults.Add(driver);
    }

    public (ulong userId, int tenthPlacePoints)[] MakeTenthPlaceResults()
    {
        if (raceResults.Count != 20)
        {
            throw new ArgumentException($"{nameof(raceResults)} should have exactly 20 elements");
        }

        var userPointsDistribution = tenthPlaceUserToDriverIndex.Keys.ToDictionary(x => x, _ => 0);
        for (var i = 0; i < raceResults.Count; i++)
        {
            var position = i + 1;
            var usersPredictedThisDriver = tenthPlaceDriverToUsersIndex.TryGetValue(raceResults[i], out var users) ? users : new List<ulong>();
            if (usersPredictedThisDriver.Count == 0)
            {
                continue;
            }

            foreach (var user in usersPredictedThisDriver)
            {
                userPointsDistribution[user] = PointsDistribution[position];
            }
        }

        return userPointsDistribution.Select(kv => (kv.Key, kv.Value)).ToArray();
    }

    public (ulong userId, int tenthPlacePoints)[]? MakeFirstDnfResults(F1Driver firstDnf)
    {
        var userPointsDistribution = firstDnfUserToDriverIndex.Keys.ToDictionary(x => x, _ => 0);
        var usersPredictedThisDriver = firstDnfDriverToUsersIndex.TryGetValue(firstDnf, out var users) ? users : new List<ulong>();
        if (usersPredictedThisDriver.Count == 0)
        {
            return null;
        }

        foreach (var user in usersPredictedThisDriver)
        {
            userPointsDistribution[user] = 5;
        }

        return userPointsDistribution.Select(kv => (kv.Key, kv.Value)).ToArray();
    }

    public F1Driver[] DriversToAddToResult()
    {
        return Enum.GetValues<F1Driver>().Except(raceResults).ToArray();
    }

    private static readonly Dictionary<int, int> PointsDistribution = new()
    {
        {1, 1},
        {2, 2},
        {3, 4},
        {4, 6},
        {5, 8},
        {6, 10},
        {7, 12},
        {8, 15},
        {9, 18},
        {10, 25},
        {11, 18},
        {12, 15},
        {13, 12},
        {14, 10},
        {15, 8},
        {16, 6},
        {17, 4},
        {18, 2},
        {19, 1},
        {20, 1},
    };

    private readonly List<F1Driver> raceResults;
    private readonly Dictionary<ulong, F1Driver> tenthPlaceUserToDriverIndex;
    private readonly Dictionary<ulong, F1Driver> firstDnfUserToDriverIndex;
    private readonly Dictionary<F1Driver, List<ulong>> tenthPlaceDriverToUsersIndex;
    private readonly Dictionary<F1Driver, List<ulong>> firstDnfDriverToUsersIndex;
}