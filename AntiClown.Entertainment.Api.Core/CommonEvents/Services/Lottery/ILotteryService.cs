using AntiClown.Entertainment.Api.Core.CommonEvents.Domain.Lottery;

namespace AntiClown.Entertainment.Api.Core.CommonEvents.Services.Lottery;

public interface ILotteryService : ICommonEventServiceBase<LotteryEvent>
{
    Task AddParticipantAsync(Guid eventId, Guid userId);
    Task FinishAsync(Guid eventId);
}