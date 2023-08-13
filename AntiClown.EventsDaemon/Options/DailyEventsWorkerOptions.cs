namespace AntiClown.EventsDaemon.Options;

public class DailyEventsWorkerOptions
{
    public int StartHour { get; set; }
    public int StartMinute { get; set; }
    public TimeSpan IterationTime { get; set; }
}