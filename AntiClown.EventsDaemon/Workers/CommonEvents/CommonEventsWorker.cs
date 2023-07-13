using AntiClown.EntertainmentApi.Client;
using AntiClown.EntertainmentApi.Dto.CommonEvents;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.EventsDaemon.Workers.CommonEvents;

public class CommonEventsWorker : PeriodicJobWorker
{
    public CommonEventsWorker(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        ILogger<CommonEventsWorker> logger
    ) : base(logger, ShortIterationTime)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
    }

    protected override int CalculateTimeBeforeStart()
    {
        // calculate time for scheduler start
        // events will start at the half of odd hours
        // example: 9:30, 11:30, 13:30 etc.
        const int eventStartHour = 1;    // 1 == odd, 0 = even
        const int eventStartMinute = 30; // xx:30 minutes
        var now = DateTime.Now;
        var secondsToSleep = 60 - now.Second;
        var minutesToSleep = now.Minute < eventStartMinute
            ? (eventStartMinute - now.Minute - 1)
            : (60 - now.Minute + eventStartMinute - 1);
        var hoursToSleep = now.Hour % 2 == eventStartHour
            ? (now.Minute < eventStartMinute ? 0 : 1)
            : (now.Minute < eventStartMinute ? 1 : 0);

        return (hoursToSleep * 60 * 60 + minutesToSleep * 60 + secondsToSleep) * 1000;
    }

    protected override async Task ExecuteIterationAsync()
    {
        var activeEvents = await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.ReadActiveEventsAsync();
        if (activeEvents.Length == 0)
        {
            return;
        }

        var eventToExecute = activeEvents.SelectRandomItem();
        var eventId = eventToExecute switch
        {
            CommonEventTypeDto.GuessNumber => await antiClownEntertainmentApiClient.CommonEvents.GuessNumber
                .StartNewAsync(),
            CommonEventTypeDto.Lottery => await antiClownEntertainmentApiClient.CommonEvents.Lottery.StartNewAsync(),
            CommonEventTypeDto.Race => await antiClownEntertainmentApiClient.CommonEvents.Race.StartNewAsync(),
            CommonEventTypeDto.RemoveCoolDowns => await antiClownEntertainmentApiClient.CommonEvents.RemoveCoolDowns
                .StartNewAsync(),
            CommonEventTypeDto.Transfusion => await antiClownEntertainmentApiClient.CommonEvents.Transfusion
                .StartNewAsync(),
            CommonEventTypeDto.Bedge => await antiClownEntertainmentApiClient.CommonEvents.Bedge.StartNewAsync(),
            _ => throw new ArgumentOutOfRangeException(
                nameof(eventToExecute),
                $"Found unknown for {WorkerName} type of event. Consider adding an execution path for this type of event."
            )
        };
        Logger.LogInformation("{WorkerName} started {eventType} event with id {eventId}", WorkerName, eventToExecute,
            eventId);
    }

    protected override string WorkerName => nameof(CommonEventsWorker);

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;

    /// <summary>
    ///     Real worker timespan
    /// </summary>
    private static readonly TimeSpan RealWorkerIterationTime =
        new(days: 0, hours: 2, minutes: 0, seconds: 0, milliseconds: 500);

    /// <summary>
    ///     Worker timespan for tests
    /// </summary>
    private static readonly TimeSpan ShortIterationTime =
        new(days: 0, hours: 0, minutes: 1, seconds: 30, milliseconds: 0);
}