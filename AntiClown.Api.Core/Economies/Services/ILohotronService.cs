using AntiClown.Api.Core.Economies.Domain;

namespace AntiClown.Api.Core.Economies.Services;

public interface ILohotronService
{
    Task<LohotronReward> UseLohotronAsync(Guid userId);
    Task ResetLohotronForAllAsync();
}