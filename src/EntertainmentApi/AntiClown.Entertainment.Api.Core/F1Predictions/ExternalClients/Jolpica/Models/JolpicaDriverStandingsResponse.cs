using Newtonsoft.Json;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica.Models;

public record JolpicaDriverStandingsResponse
{
    [JsonProperty("MRData")]
    public JolpicaStandingsMrData? MrData { get; set; }
}

public record JolpicaStandingsMrData
{
    [JsonProperty("StandingsTable")]
    public JolpicaStandingsTable? StandingsTable { get; set; }
}

public record JolpicaStandingsTable
{
    [JsonProperty("StandingsLists")]
    public JolpicaStandingsList[]? StandingsLists { get; set; }
}

public record JolpicaStandingsList
{
    [JsonProperty("round")]
    public string? Round { get; set; }

    [JsonProperty("DriverStandings")]
    public JolpicaDriverStanding[]? DriverStandings { get; set; }
}

public record JolpicaDriverStanding
{
    [JsonProperty("position")]
    public string Position { get; set; } = string.Empty;

    [JsonProperty("Driver")]
    public JolpicaDriver Driver { get; set; } = new();
}
