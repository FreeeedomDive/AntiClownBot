using AntiClown.Api.Client;
using AntiClown.Core.Schedules;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain;
using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.Entertainment.Api.Core.CommonEvents.Services.Messages;
using AntiClown.Entertainment.Api.Dto.Exceptions.CommonEvents;
using Hangfire;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.GuessNumber;

public class GuessNumberEventService : IGuessNumberEventService
{
    public GuessNumberEventService(
        IAntiClownApiClient antiClownApiClient,
        ICommonEventsRepository commonEventsRepository,
        ICommonEventsMessageProducer commonEventsMessageProducer,
        IAntiClownDataApiClient antiClownDataApiClient,
        IScheduler scheduler
    )
    {
        this.antiClownApiClient = antiClownApiClient;
        this.commonEventsRepository = commonEventsRepository;
        this.commonEventsMessageProducer = commonEventsMessageProducer;
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.scheduler = scheduler;
    }

    public async Task<GuessNumberEvent> ReadAsync(Guid eventId)
    {
        var @event = await commonEventsRepository.ReadAsync(eventId);
        if (@event.Type != CommonEventType.GuessNumber)
        {
            throw new WrongEventTypeException($"Event {eventId} is not a guess number");
        }

        return (@event as GuessNumberEvent)!;
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var newEvent = GuessNumberEvent.Create();
        await commonEventsRepository.CreateAsync(newEvent);
        await commonEventsMessageProducer.ProduceAsync(newEvent);
        await ScheduleEventFinishAsync(newEvent.Id);

        return newEvent.Id;
    }

    public async Task AddParticipantAsync(Guid eventId, Guid userId, GuessNumberPick userPick)
    {
        var @event = await ReadAsync(eventId);
        if (@event.Finished)
        {
            throw new EventAlreadyFinishedException(eventId);
        }

        @event.AddPick(userId, userPick);
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

        var winners = @event.NumberToUsers.TryGetValue(@event.Result, out var result) ? result : new List<Guid>();
        foreach (var winnerUserId in winners)
        {
            await antiClownApiClient.Economy.UpdateLootBoxesAsync(winnerUserId, 1);
        }

        await commonEventsMessageProducer.ProduceAsync(@event);
    }

    private async Task ScheduleEventFinishAsync(Guid eventId)
    {
        var delayInMilliseconds = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.CommonEvents, "GuessNumberEvent.WaitingTimeInMilliseconds");
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

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly ICommonEventsMessageProducer commonEventsMessageProducer;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly ICommonEventsRepository commonEventsRepository;
    private readonly IScheduler scheduler;
}