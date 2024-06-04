/* Generated file */
namespace AntiClown.Entertainment.Api.Client.GuessNumberEvent;

public interface IGuessNumberEventClient
{
    System.Threading.Tasks.Task<AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberEventDto> ReadAsync(System.Guid eventId);
    System.Threading.Tasks.Task<System.Guid> StartNewAsync();
    System.Threading.Tasks.Task AddPickAsync(System.Guid eventId, AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber.GuessNumberUserPickDto guessNumberUserPickDto);
    System.Threading.Tasks.Task FinishAsync(System.Guid eventId);
}
