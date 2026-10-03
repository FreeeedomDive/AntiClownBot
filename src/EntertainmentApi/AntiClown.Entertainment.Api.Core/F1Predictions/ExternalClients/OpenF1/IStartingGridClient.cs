namespace AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.OpenF1;

public interface IStartingGridClient
{
    Task<string[]?> GetDriverNamesAsync(int season, int raceIndex, bool isSprint);
}
