using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Dto.DailyEvents;
using AntiClown.EventsDaemon.Options;
using Microsoft.Extensions.Options;

namespace AntiClown.EventsDaemon.Workers.DailyEvents;

public class DailyEventsWorker : PeriodicJobWorker
{
    public DailyEventsWorker(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IOptions<DailyEventsWorkerOptions> dailyEventsWorkerOptions,
        ILogger<DailyEventsWorker> logger
    ) : base(logger, dailyEventsWorkerOptions.Value.IterationTime)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        options = dailyEventsWorkerOptions.Value;
    }

    protected override int CalculateTimeBeforeStart()
    {
        // edit this
        // TODO to settings
        var dailyEventStartHour = options.StartHour;
        var dailyEventStartMinute = options.StartMinute;

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
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;

    private readonly DailyEventsWorkerOptions options;
}