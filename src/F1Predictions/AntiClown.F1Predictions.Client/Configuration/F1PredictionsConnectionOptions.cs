namespace AntiClown.F1Predictions.Client.Configuration;

public class F1PredictionsConnectionOptions
{
    public string ServiceUrl { get; set; } = "";
    public string ServiceKey { get; set; } = "";
    public Guid LeagueId { get; set; }
    public int TimeoutMilliseconds { get; set; } = 10000;
}
