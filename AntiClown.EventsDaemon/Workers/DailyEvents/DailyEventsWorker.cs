namespace AntiClown.EventsDaemon.Workers.DailyEvents;

public class DailyEventsWorker : PeriodicJobWorker
{
    public DailyEventsWorker(
        ILogger<DailyEventsWorker> logger
    ) : base(logger, ShortIterationTime)
    {
    }

    protected override int CalculateTimeBeforeStart()
    {
        // edit this
        const int dailyEventStartHour = 24;
        const int dailyEventStartMinute = 00;

        const int dailyEventStartHourUtc = dailyEventStartHour - 5;
        var now = DateTime.UtcNow;
        var diff = new TimeSpan(
            hours: now.Hour >= dailyEventStartHourUtc
                ? 24 - (now.Hour - dailyEventStartHourUtc) - 1
                : dailyEventStartHourUtc - now.Hour - 1,
            minutes: now.Minute >= dailyEventStartMinute
                ? 60 - (now.Minute - dailyEventStartMinute)
                : dailyEventStartMinute - now.Minute,
            seconds: 0
        );

        return (int)diff.TotalMilliseconds;
    }

    protected override Task ExecuteIterationAsync()
    {
        throw new NotImplementedException();
    }

    protected override string WorkerName => nameof(DailyEventsWorker);

    /// <summary>
    ///     Real worker timespan
    /// </summary>
    private static readonly TimeSpan RealWorkerIterationTime =
        new(days: 0, hours: 2, minutes: 0, seconds: 0, milliseconds: 500);

    /// <summary>
    ///     Worker timespan for tests
    /// </summary>
    private static readonly TimeSpan ShortIterationTime =
        new(days: 0, hours: 0, minutes: 5, seconds: 00, milliseconds: 0);
}