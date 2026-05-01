namespace AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;

public interface IJolpicaClient
{
    Task<string[]?> GetQualifyingDriverNamesAsync(int season, int raceIndex);
    Task<(int Round, string[] Standings)?> GetDriverStandingsAsync(int season);
}
