using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;

public class JolpicaClient(ILogger<JolpicaClient> logger) : IJolpicaClient
{
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("https://api.jolpi.ca"),
        Timeout = TimeSpan.FromSeconds(30),
    };

    public async Task<string[]?> GetQualifyingDriverNamesAsync(int season, int raceIndex)
    {
        var url = $"/ergast/f1/{season}/{raceIndex}/qualifying.json";
        var response = await HttpClient.GetAsync(url);

        logger.LogInformation(
            "Jolpica qualifying response for race {RaceIndex} season {Season}: {StatusCode}",
            raceIndex, season, response.StatusCode
        );

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Jolpica returned non-success status {Status}", response.StatusCode);
            return null;
        }

        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<JolpicaQualifyingResponse>(content);
        var results = data?.MrData?.RaceTable?.Races?.FirstOrDefault()?.QualifyingResults;

        if (results is null || results.Length == 0)
        {
            logger.LogWarning(
                "Jolpica returned no qualifying results for race {RaceIndex} season {Season}",
                raceIndex, season
            );
            return null;
        }

        return results
            .OrderBy(r => int.TryParse(r.Position, out var pos) ? pos : 99)
            .Select(r => CapitalizeDriverId(r.Driver.DriverId))
            .ToArray();
    }

    private static string CapitalizeDriverId(string driverId)
    {
        var last = driverId.Split('_').Last();
        return char.ToUpperInvariant(last[0]) + last[1..];
    }
}
