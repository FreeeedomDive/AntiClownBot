using AntiClown.Api.Core.Economies.Domain;

namespace AntiClown.Api.Core.Economies.Services;

public interface ILohotronRewardGenerator
{
    LohotronReward Generate();
}