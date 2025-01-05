using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.EventsDaemon.Workers.CommonEvents;

public class CommonEventsWorker : FixedIntervalPeriodicJobWorker
{
    public CommonEventsWorker(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IAntiClownDataApiClient antiClownDataApiClient,
        ILogger<CommonEventsWorker> logger
    ) : base(logger)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.antiClownDataApiClient = antiClownDataApiClient;
        IterationTime = antiClownDataApiClient.Settings.ReadAsync<TimeSpan>(SettingsCategory.CommonEvents, "CommonEventsWorker.IterationTime").GetAwaiter().GetResult();
    }

    protected override async Task<int> GetMillisecondsBeforeStartAsync()
    {
        var eventStartHour = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.CommonEvents, "CommonEventsWorker.StartHour");
        var eventStartMinute = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.CommonEvents, "CommonEventsWorker.StartMinute");
        var now = DateTime.Now;
        var secondsToSleep = 60 - now.Second;
        var minutesToSleep = now.Minute < eventStartMinute
            ? eventStartMinute - now.Minute - 1
            : 60 - now.Minute + eventStartMinute - 1;
        var hoursToSleep = now.Hour % 2 == eventStartHour
            ? now.Minute < eventStartMinute ? 0 : 1
            : now.Minute < eventStartMinute
                ? 1
                : 0;

        return (hoursToSleep * 60 * 60 + minutesToSleep * 60 + secondsToSleep) * 1000;
    }

    protected override async Task ExecuteIterationAsync()
    {
        var activeEvents = await antiClownEntertainmentApiClient.ActiveEventsIndex.ReadActiveEventsAsync();
        if (activeEvents.Length == 0)
        {
            return;
        }

        var eventToExecute = activeEvents.SelectRandomItem();
        var eventId = eventToExecute switch
        {
            CommonEventTypeDto.GuessNumber => await antiClownEntertainmentApiClient.GuessNumberEvent.StartNewAsync(),
            CommonEventTypeDto.Lottery => await antiClownEntertainmentApiClient.LotteryEvent.StartNewAsync(),
            CommonEventTypeDto.Race => await antiClownEntertainmentApiClient.RaceEvent.StartNewAsync(),
            CommonEventTypeDto.RemoveCoolDowns => await antiClownEntertainmentApiClient.RemoveCoolDownsEvent.StartNewAsync(),
            CommonEventTypeDto.Transfusion => await antiClownEntertainmentApiClient.TransfusionEvent.StartNewAsync(),
            CommonEventTypeDto.Bedge => await antiClownEntertainmentApiClient.BedgeEvent.StartNewAsync(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(eventToExecute),
                $"Found unknown for {WorkerName} type of event. Consider adding an execution path for this type of event."
            ),
        };
        Logger.LogInformation(
            "{WorkerName} started {eventType} event with id {eventId}", WorkerName, eventToExecute,
            eventId
        );
    }

    protected sealed override TimeSpan IterationTime { get; set; }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}