namespace AntiClownApiClient.Dto.Settings;

public class EventSettings
{
    public string EventsType { get; set; }
    public int EventIntervalInMinutes { get; set; }
    public int LotteryStartDelayInMinutes { get; set; }
    public int RaceStartDelayInMinutes { get; set; }
}