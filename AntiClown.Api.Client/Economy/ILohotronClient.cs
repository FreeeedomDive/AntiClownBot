using AntiClown.Api.Dto.Economies;

namespace AntiClown.Api.Client.Economy;

public interface ILohotronClient
{
    Task<LohotronRewardDto> UseLohotronAsync(Guid userId);
    Task ResetLohotronForAllUsersAsync();
}