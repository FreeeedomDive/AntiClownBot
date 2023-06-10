using AntiClown.Api.Dto.Economies;

namespace AntiClown.Api.Client.Economy;

public interface ITributeClient
{
    Task<NextTributeDto> ReadNextTributeInfoAsync(Guid userId);
    Task<TributeDto> TributeAsync(Guid userId);
}