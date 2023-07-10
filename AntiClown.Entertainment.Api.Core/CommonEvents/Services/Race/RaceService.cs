using AntiClown.Api.Client;
using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.Entertainment.Api.Core.Common;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Race;

public class RaceService : IRaceService
{
    public RaceService(
        ICommonEventsRepository commonEventsRepository,
        IRaceDriversRepository raceDriversRepository,
        IRaceTracksRepository raceTracksRepository,
        IRaceGenerator raceGenerator,
        IEventsMessageProducer eventsMessageProducer,
        IAntiClownApiClient antiClownApiClient,
        IScheduler scheduler
    )
    {
        this.commonEventsRepository = commonEventsRepository;
        this.raceDriversRepository = raceDriversRepository;
        this.raceTracksRepository = raceTracksRepository;
        this.raceGenerator = raceGenerator;
        this.eventsMessageProducer = eventsMessageProducer;
        this.antiClownApiClient = antiClownApiClient;
        this.scheduler = scheduler;
    }

    public async Task<RaceEvent> ReadAsync(Guid eventId)
    {
        var @event = await commonEventsRepository.ReadAsync(eventId);
        if (@event.Type != CommonEventType.Race)
        {
            throw new WrongEventTypeException($"Event {eventId} is not a race");
        }

        return (@event as RaceEvent)!;
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var race = await raceGenerator.GenerateAsync();
        await commonEventsRepository.CreateAsync(race);
        await eventsMessageProducer.ProduceAsync(race);
        ScheduleEventFinish(race.Id);

        return race.Id;
    }

    public async Task AddParticipantAsync(Guid eventId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task FinishAsync(Guid eventId)
    {
        throw new NotImplementedException();
    }

    private void ScheduleEventFinish(Guid eventId)
    {
        scheduler.Schedule(
            () => BackgroundJob.Schedule(
                () => FinishAsync(eventId),
                TimeSpan.FromMilliseconds(Constants.LotteryEventWaitingTimeInMilliseconds))
        );
    }

    private readonly ICommonEventsRepository commonEventsRepository;
    private readonly IRaceDriversRepository raceDriversRepository;
    private readonly IRaceTracksRepository raceTracksRepository;
    private readonly IRaceGenerator raceGenerator;
    private readonly IEventsMessageProducer eventsMessageProducer;
    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IScheduler scheduler;
}