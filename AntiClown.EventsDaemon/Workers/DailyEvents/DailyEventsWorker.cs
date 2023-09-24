using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.DailyEvents;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.EventsDaemon.Workers.DailyEvents;

public class DailyEventsWorker : PeriodicJobWorker
{
    public DailyEventsWorker(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IAntiClownDataApiClient antiClownDataApiClient,
        ILoggerClient loggerClient
    ) : base(loggerClient)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.antiClownDataApiClient = antiClownDataApiClient;
        IterationTime = antiClownDataApiClient.Settings.ReadAsync<TimeSpan>(SettingsCategory.DailyEvents, "DailyEventsWorker.IterationTime").GetAwaiter().GetResult();
    }

    protected override async Task<int> CalculateTimeBeforeStartAsync()
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

    protected sealed override TimeSpan IterationTime { get; set; }
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
}