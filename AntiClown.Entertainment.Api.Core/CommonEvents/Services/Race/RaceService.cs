using AntiClown.Api.Client;
using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.Entertainment.Api.Core.Common;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.EntertainmentApi.Dto.CommonEvents.Race;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;
using AntiClown.Tools.Utility.Random;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Race;

public class RaceService : IRaceService
{
    public RaceService(
        ICommonEventsRepository commonEventsRepository,
        IRaceDriversRepository raceDriversRepository,
        IRaceGenerator raceGenerator,
        IEventsMessageProducer eventsMessageProducer,
        IAntiClownApiClient antiClownApiClient,
        IScheduler scheduler
    )
    {
        this.commonEventsRepository = commonEventsRepository;
        this.raceDriversRepository = raceDriversRepository;
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
        var @event = await ReadAsync(eventId);
        if (@event.Finished)
        {
            throw new EventAlreadyFinishedException(eventId);
        }

        @event.AddParticipant(userId);
        await commonEventsRepository.UpdateAsync(@event);
    }

    public async Task FinishAsync(Guid eventId)
    {
        var @event = await ReadAsync(eventId);
        if (@event.Finished)
        {
            throw new EventAlreadyFinishedException(eventId);
        }

        @event.Finished = true;
        await commonEventsRepository.UpdateAsync(@event);

        var driversByName = @event.Participants.ToDictionary(x => x.Driver.DriverName);
        var startSectorPositions = GetOrderedDriversFromSector(@event.Sectors.First())
                .Select((x, i) => new { Name = x.DriverName, Position = i })
                .ToDictionary(x => x.Name, x => x.Position);
        var finishSectorPositions = GetOrderedDriversFromSector(@event.Sectors.Last())
            .Select((x, i) => new { Name = x.DriverName, Position = i })
            .ToArray();

        foreach (var t in finishSectorPositions)
        {
            var points = RaceHelper.PositionToPoints.TryGetValue(t.Position, out var x) ? x : 0;
            var driver = driversByName[t.Name];
            if (driver.UserId is not null && points > 0)
            {
                var scamCoins = points * RaceHelper.PointsToScamCoinsMultiplier;
                await antiClownApiClient.Economy.UpdateScamCoinsAsync(driver.UserId.Value, scamCoins,$"Гонка {eventId}");
            }

            var diff = t.Position - startSectorPositions[t.Name];
            ApplySkillResults(diff, driver.Driver);

            if (points > 0 && diff > 0)
            {
                await raceDriversRepository.UpdateAsync(driver.Driver);
            }
        }
    }

    private static IEnumerable<RaceSnapshotForDriverOnSector> GetOrderedDriversFromSector(RaceSnapshotOnSector sector)
    {
        return sector
            .DriversOnSector
            .OrderBy(x => x.TotalTime);
    }

    private static void ApplySkillResults(int skillsToImprove, RaceDriver raceDriver)
    {
        for (var i = 0; i < skillsToImprove; i++)
        {
            switch (Randomizer.GetRandomNumberBetween(0, 3))
            {
                case 0:
                    raceDriver.CorneringSkill += 0.001f;
                    break;
                case 1:
                    raceDriver.AccelerationSkill += 0.001f;
                    break;
                case 2:
                    raceDriver.BreakingSkill += 0.001f;
                    break;
            }
        }
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
    private readonly IRaceGenerator raceGenerator;
    private readonly IEventsMessageProducer eventsMessageProducer;
    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IScheduler scheduler;
}