using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Dto.DailyEvents;

namespace AntiClown.EventsDaemon.Workers.DailyEvents;

public class DailyEventsWorker : PeriodicJobWorker
{
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;

    public DailyEventsWorker(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<DailyEventsWorker> logger
    ) : base(logger, ShortIterationTime)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override int CalculateTimeBeforeStart()
    {
        // edit this
        // TODO to settings
        const int dailyEventStartHour = 00;
        const int dailyEventStartMinute = 00;

        const int utcDiff = 5;

        var nowUtc = DateTime.UtcNow;
        var scheduledTime = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, 24 + dailyEventStartHour - utcDiff, dailyEventStartMinute, 0);
        if (nowUtc > scheduledTime)
        {
            scheduledTime = scheduledTime.AddDays(1);
        }

        return (int)(scheduledTime - nowUtc).TotalMilliseconds;
    }

    protected override async Task ExecuteIterationAsync()
    {
        var activeEvents = await antiClownEntertainmentApiClient.DailyEvents.ActiveDailyEventsIndex.ReadActiveEventsAsync();
        if (activeEvents.Length == 0)
        {
            return;
        }

        foreach (var activeEventType in activeEvents)
        {
            switch (activeEventType)
            {
                case DailyEventTypeDto.Announce:
                    await antiClownEntertainmentApiClient.DailyEvents.Announce.StartNewAsync();
                    break;
                case DailyEventTypeDto.PaymentsAndResets:
                    await antiClownEntertainmentApiClient.DailyEvents.PaymentsAndResets.StartNewAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(activeEventType),
                        $"Found unknown for {WorkerName} type of event. Consider adding an execution path for this type of event."
                    );
            }
        }
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