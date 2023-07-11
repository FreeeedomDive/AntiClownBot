using AntiClown.EntertainmentApi.Dto.CommonEvents.GuessNumber;

namespace AntiClown.EntertainmentApi.Client.CommonEvents.GuessNumber;

public interface IGuessNumberEventClient
{
    Task<GuessNumberEventDto> ReadAsync(Guid eventId);
    Task<Guid> StartNewAsync();
    Task AddPickAsync(Guid eventId, Guid userId, GuessNumberPickDto pick);
    Task FinishAsync(Guid eventId);
}