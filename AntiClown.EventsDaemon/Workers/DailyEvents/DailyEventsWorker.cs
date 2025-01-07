using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.DailyEvents;

namespace AntiClown.EventsDaemon.Workers.DailyEvents;

public class DailyEventsWorker(
    IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
    IAntiClownDataApiClient antiClownDataApiClient,
    ILogger<DailyEventsWorker> logger
)
    : FixedIntervalPeriodicJobWorker(logger)
{
    protected override async Task<int> GetMillisecondsBeforeStartAsync()
    {
        var dailyEventStartHour = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.DailyEvents, "DailyEventsWorker.StartHour");
        var dailyEventStartMinute = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.DailyEvents, "DailyEventsWorker.StartMinute");

        const int utcDiff = 5;

        var nowUtc = DateTime.UtcNow;
        var scheduledTime = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, (24 + dailyEventStartHour - utcDiff) % 24, dailyEventStartMinute, 0);
        if (nowUtc > scheduledTime)
        {
            scheduledTime = scheduledTime.AddDays(1);
        }

        return (int)(scheduledTime - nowUtc).TotalMilliseconds;
    }

    protected override async Task ExecuteIterationAsync()
    {
        var activeEvents = await antiClownEntertainmentApiClient.ActiveDailyEventsIndex.ReadActiveEventsAsync();
        if (activeEvents.Length == 0)
        {
            return;
        }

        foreach (var activeEventType in activeEvents)
        {
            switch (activeEventType)
            {
                case DailyEventTypeDto.Announce:
                    await antiClownEntertainmentApiClient.AnnounceEvent.StartNewAsync();
                    break;
                case DailyEventTypeDto.PaymentsAndResets:
                    await antiClownEntertainmentApiClient.PaymentsAndResetsEvent.StartNewAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(activeEventType),
                        $"Found unknown for {WorkerName} type of event. Consider adding an execution path for this type of event."
                    );
            }
        }
    }

    protected sealed override TimeSpan IterationTime { get; set; } = antiClownDataApiClient.Settings.ReadAsync<TimeSpan>(SettingsCategory.DailyEvents, "DailyEventsWorker.IterationTime").GetAwaiter().GetResult();
}