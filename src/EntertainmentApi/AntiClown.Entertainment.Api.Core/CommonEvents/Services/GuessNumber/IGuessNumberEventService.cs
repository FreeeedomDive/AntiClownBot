using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.GuessNumber;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.GuessNumber;

public interface IGuessNumberEventService : ICommonEventServiceBase<GuessNumberEvent>
{
    Task AddParticipantAsync(Guid eventId, Guid userId, GuessNumberPick userPick);
    Task FinishAsync(Guid eventId);
}