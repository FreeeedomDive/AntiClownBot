using AntiClownApiClient.Dto.Responses.UserCommandResponses;

namespace AntiClownApiClient.UtilityClient;

public interface IUtilityClient
{
    Task<List<TributeResponseDto>> GetAutomaticTributesAsync();
    Task<bool> PingApiAsync();
}