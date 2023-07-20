using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.CommonEvents;
using AntiClown.EventsDaemon.Options;
using AntiClown.Tools.Utility.Extensions;
using Microsoft.Extensions.Options;
using TelemetryApp.Api.Client.Log;

namespace AntiClown.EventsDaemon.Workers.CommonEvents;

public class CommonEventsWorker : PeriodicJobWorker
{
    public CommonEventsWorker(
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IOptions<CommonEventsWorkerOptions> commonEventsWorkerOptions,
        ILoggerClient loggerClient
    ) : base(loggerClient, commonEventsWorkerOptions.Value.IterationTime)
    {
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        options = commonEventsWorkerOptions.Value;
    }

    protected override int CalculateTimeBeforeStart()
    {
        var eventStartHour = options.StartHour;
        var eventStartMinute = options.StartMinute;
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
        var activeEvents =
            await antiClownEntertainmentApiClient.CommonEvents.ActiveCommonEventsIndex.ReadActiveEventsAsync();
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
            ),
        };
        await Logger.InfoAsync(
            "{WorkerName} started {eventType} event with id {eventId}", WorkerName, eventToExecute,
            eventId
        );
    }

    protected override string WorkerName => nameof(CommonEventsWorker);
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;

    private readonly CommonEventsWorkerOptions options;
}