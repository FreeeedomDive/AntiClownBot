namespace AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;

public interface IJolpicaClient
{
    Task<(int Round, string[] Standings)?> GetDriverStandingsAsync(int season);
}
