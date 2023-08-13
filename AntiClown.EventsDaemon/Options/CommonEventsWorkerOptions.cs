namespace AntiClown.EventsDaemon.Options;

public class CommonEventsWorkerOptions
{
    public int StartHour { get; set; }
    public int StartMinute { get; set; }
    public TimeSpan IterationTime { get; set; }
}