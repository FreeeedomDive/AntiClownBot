using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;
using AntiClown.Entertainment.Api.Core.CommonEvents.Repositories;
using AntiClown.EntertainmentApi.Dto.Exceptions.CommonEvents;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services;

public class GuessNumberEventService : IGuessNumberEventService
{
    public GuessNumberEventService(ICommonEventsRepository commonEventsRepository)
    {
        this.commonEventsRepository = commonEventsRepository;
    }

    public async Task<Guid> StartNewEventAsync()
    {
        var newEvent = new GuessNumberEvent
        {
            Id = Guid.NewGuid(),
            Finished = false,
            EventDateTime = DateTime.UtcNow,
            Picks = new Dictionary<Guid, GuessNumberPick>(),
            NumberToUsers = new Dictionary<GuessNumberPick, List<Guid>>(),
            Result = Enum.GetValues<GuessNumberPick>().SelectRandomItem()
        };
        await commonEventsRepository.CreateAsync(newEvent);

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

    public async Task<GuessNumberEvent> ReadAsync(Guid eventId)
    {
        return (await commonEventsRepository.ReadAsync(eventId) as GuessNumberEvent)!;
    }

    private readonly ICommonEventsRepository commonEventsRepository;
}