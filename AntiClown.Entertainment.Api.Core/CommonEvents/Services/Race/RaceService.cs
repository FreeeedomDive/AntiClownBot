﻿using AntiClown.Api.Client;
using AntiClown.Api.Dto.Economies;
using AntiClown.Core.Schedules;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Domain;
using AntiClown.Entertainment.Api.Core.AdditionalEventsInfo.Race.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Race;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;
using AntiClown.Tools.Utility.Random;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Race;

public class RaceService(
    ICommonEventsRepository commonEventsRepository,
    IRaceDriversRepository raceDriversRepository,
    IRaceGenerator raceGenerator,
    ICommonEventsMessageProducer commonEventsMessageProducer,
    IAntiClownApiClient antiClownApiClient,
    IAntiClownDataApiClient antiClownDataApiClient,
    IScheduler scheduler
) : IRaceService
{
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
        await commonEventsMessageProducer.ProduceAsync(race);
        await ScheduleEventFinishAsync(race.Id);

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
                                   .Select((x, i) => new { Name = x.DriverName, Position = i + 1 })
                                   .ToDictionary(x => x.Name, x => x.Position);
        var finishSectorPositions = GetOrderedDriversFromSector(@event.Sectors.Last())
                                    .Select((x, i) => new { Name = x.DriverName, Position = i + 1 })
                                    .ToArray();

        foreach (var t in finishSectorPositions)
        {
            var points = RaceHelper.PositionToPoints.GetValueOrDefault(t.Position, 0);
            var driver = driversByName[t.Name];
            if (driver.UserId is not null && points > 0)
            {
                var scamCoins = points * RaceHelper.PointsToScamCoinsMultiplier;
                await antiClownApiClient.Economy.UpdateScamCoinsAsync(
                    driver.UserId.Value, new UpdateScamCoinsDto
                    {
                        UserId = driver.UserId.Value,
                        ScamCoinsDiff = scamCoins,
                        Reason = $"Гонка {eventId}",
                    }
                );
            }

            var diff = t.Position - startSectorPositions[t.Name];
            ApplyResultsToDriver(driver.Driver, diff, points);

            if (points > 0 || diff > 0)
            {
                await raceDriversRepository.UpdateAsync(driver.Driver);
            }
        }

        await commonEventsMessageProducer.ProduceAsync(@event);
    }

    private static IEnumerable<RaceSnapshotForDriverOnSector> GetOrderedDriversFromSector(RaceSnapshotOnSector sector)
    {
        return sector
               .DriversOnSector
               .OrderBy(x => x.TotalTime);
    }

    private static void ApplyResultsToDriver(RaceDriver raceDriver, int skillsToImprove, int points)
    {
        raceDriver.Points += points;
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

    private async Task ScheduleEventFinishAsync(Guid eventId)
    {
        var delayInMilliseconds = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.CommonEvents, "RaceEvent.WaitingTimeInMilliseconds");
        scheduler.Schedule(
            () => BackgroundJob.Schedule(
                () => SafeFinishAsync(eventId),
                TimeSpan.FromMilliseconds(delayInMilliseconds)
            )
        );
    }

    // keep this method public for Hangfire
    public async Task SafeFinishAsync(Guid eventId)
    {
        try
        {
            await FinishAsync(eventId);
        }
        catch (EventAlreadyFinishedException)
        {
        }
    }
}