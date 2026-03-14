using Newtonsoft.Json;

namespace AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica.Models;

public record JolpicaQualifyingResponse
{
    [JsonProperty("MRData")]
    public JolpicaMrData? MrData { get; set; }
}

public record JolpicaMrData
{
    [JsonProperty("RaceTable")]
    public JolpicaRaceTable? RaceTable { get; set; }
}

public record JolpicaRaceTable
{
    [JsonProperty("Races")]
    public JolpicaRace[]? Races { get; set; }
}

public record JolpicaRace
{
    [JsonProperty("QualifyingResults")]
    public JolpicaQualifyingResult[]? QualifyingResults { get; set; }
}

public record JolpicaQualifyingResult
{
    [JsonProperty("position")]
    public string Position { get; set; } = string.Empty;

    [JsonProperty("Driver")]
    public JolpicaDriver Driver { get; set; } = new();
}

public record JolpicaDriver
{
    [JsonProperty("driverId")]
    public string DriverId { get; set; } = string.Empty;
}
