using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Lottery;

public interface ILotteryService : IBaseEventService<LotteryEvent>
{
    Task AddParticipantAsync(Guid eventId, Guid userId);
    Task<LotteryEvent> FinishAsync(Guid eventId);
}