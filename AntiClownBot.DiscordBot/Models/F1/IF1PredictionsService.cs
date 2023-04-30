namespace AntiClownDiscordBotVersion2.Models.F1;

public interface IF1PredictionsService
{
    void PredictTenthPlace(ulong userId, F1Driver f1Driver);
    void PredictDnf(ulong userId, F1Driver f1Driver);
    Dictionary<ulong, F1Driver> GetTenthPlacePredictions();
    Dictionary<ulong, F1Driver> GetFirstDnfPredictions();
    void AddPlayerToResult(F1Driver driver);
    F1Driver[] DriversToAddToResult();
    (ulong userId, int tenthPlacePoints)[]? MakeFirstDnfResults(F1Driver firstDnf);
    (ulong userId, int tenthPlacePoints)[] MakeTenthPlaceResults();
}