/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Entertainment.Api.Client.GuessNumberEvent;

public interface IGuessNumberEventClient
{
    Task<AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberEventDto> ReadAsync(System.Guid eventId);
    Task<System.Guid> StartNewAsync();
    Task AddPickAsync(System.Guid eventId, AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberUserPickDto guessNumberUserPickDto);
    Task FinishAsync(System.Guid eventId);
}
