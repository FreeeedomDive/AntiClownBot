/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.LotteryEvent;

public interface ILotteryEventClient
{
    Task<AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery.LotteryEventDto> ReadAsync(System.Guid eventId);
    Task<System.Guid> StartNewAsync();
    Task AddParticipantAsync(System.Guid eventId, System.Guid userId);
    Task FinishAsync(System.Guid eventId);
}
