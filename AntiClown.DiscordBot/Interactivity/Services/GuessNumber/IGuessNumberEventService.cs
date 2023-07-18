using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;

namespace AntiClown.DiscordBot.Interactivity.Services.GuessNumber;

public interface IGuessNumberEventService
{
    Task CreateAsync(Guid eventId);
    Task AddUserPickAsync(Guid eventId, ulong memberId, GuessNumberPickDto pick);
    Task FinishAsync(Guid eventId);
}