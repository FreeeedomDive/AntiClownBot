using AntiClown.EntertainmentApi.Dto.CommonEvents.Lottery;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.Lottery;

public interface ILotteryClient
{
    Task<LotteryEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
    Task AddParticipantAsync(Guid eventId, Guid userId);
    Task FinishAsync(Guid eventId);
}