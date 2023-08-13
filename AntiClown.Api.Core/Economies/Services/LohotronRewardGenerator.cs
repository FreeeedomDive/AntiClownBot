using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Tools.Utility.Extensions;

namespace AntiClown.Api.Core.Economies.Services;

public class LohotronRewardGenerator : ILohotronRewardGenerator
{
    public LohotronReward Generate()
    {
        var rewardWheel = new List<LohotronReward>();
        rewardWheel.AddRange(
            Enumerable.Range(0, 40).Select(
                _ => new LohotronReward
                {
                    RewardType = LohotronRewardType.Nothing,
                }
            )
        );
        rewardWheel.AddRange(
            Enumerable.Range(0, 30).Select(
                _ => new LohotronReward
                {
                    RewardType = LohotronRewardType.ScamCoins,
                    ScamCoinsReward = 1,
                }
            )
        );
        rewardWheel.AddRange(
            Enumerable.Range(0, 14).Select(
                _ => new LohotronReward
                {
                    RewardType = LohotronRewardType.ScamCoins,
                    ScamCoinsReward = 100,
                }
            )
        );
        rewardWheel.AddRange(
            Enumerable.Range(0, 7).Select(
                _ => new LohotronReward
                {
                    RewardType = LohotronRewardType.ScamCoins,
                    ScamCoinsReward = 200,
                }
            )
        );
        rewardWheel.AddRange(
            Enumerable.Range(0, 3).Select(
                _ => new LohotronReward
                {
                    RewardType = LohotronRewardType.ScamCoins,
                    ScamCoinsReward = 250,
                }
            )
        );
        rewardWheel.AddRange(
            Enumerable.Range(0, 2).Select(
                _ => new LohotronReward
                {
                    RewardType = LohotronRewardType.ScamCoins,
                    ScamCoinsReward = 500,
                }
            )
        );
        rewardWheel.AddRange(
            Enumerable.Range(0, 2).Select(
                _ => new LohotronReward
                {
                    RewardType = LohotronRewardType.ScamCoins,
                    ScamCoinsReward = -500,
                }
            )
        );
        rewardWheel.AddRange(
            Enumerable.Range(0, 6).Select(
                _ => new LohotronReward
                {
                    RewardType = LohotronRewardType.LootBox,
                }
            )
        );

        return rewardWheel.SelectRandomItem();
    }
}